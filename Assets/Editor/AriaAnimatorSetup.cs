using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.IO;

// این اسکریپت رو داخل پوشه‌ی Assets/Editor بگذار (باید حتماً اسم پوشه Editor باشه)
// بعد از قرارگیری، از منوی بالای یونیتی:
// Tools > Weaver Shadow > Setup Aria/Shadow Animator رو بزن
//
// پیش‌نیاز: اسپرایت‌های زیر باید داخل پوشه‌ی Assets/Sprites/Aria/ باشن:
// Aria_idle, Aria_walk0..7, Aria_jump, Aria_fall, Aria_duck, Aria_hurt

public class AriaAnimatorSetup : EditorWindow
{
    private const string SpriteFolder = "Assets/Sprites/Aria/";
    private const string OutputFolder = "Assets/Animations/AriaShadow/";

    [MenuItem("Tools/Weaver Shadow/Setup Aria-Shadow Animator")]
    public static void SetupAnimator()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Animations"))
            AssetDatabase.CreateFolder("Assets", "Animations");
        if (!AssetDatabase.IsValidFolder(OutputFolder.TrimEnd('/')))
            AssetDatabase.CreateFolder("Assets/Animations", "AriaShadow");

        // ۱) بارگذاری اسپرایت‌ها
        Sprite idle = LoadSprite("Aria_idle");
        Sprite[] walk = new Sprite[8];
        for (int i = 0; i < 8; i++) walk[i] = LoadSprite("Aria_walk" + i);
        Sprite jump = LoadSprite("Aria_jump");
        Sprite fall = LoadSprite("Aria_fall");
        Sprite duck = LoadSprite("Aria_duck");
        Sprite hurt = LoadSprite("Aria_hurt");

        if (idle == null || walk[0] == null)
        {
            Debug.LogError("اسپرایت‌ها پیدا نشدن! مطمئن شو فایل‌ها توی " + SpriteFolder + " هستن و اسم‌شون دقیقاً مثل Aria_idle.png باشه.");
            return;
        }

        // ۲) ساخت کلیپ‌های انیمیشن
        AnimationClip idleClip = CreateSingleFrameClip("Idle", idle, true);
        AnimationClip walkClip = CreateMultiFrameClip("Walk", walk, 12f, true);
        AnimationClip jumpClip = CreateSingleFrameClip("Jump", jump, false);
        AnimationClip fallClip = CreateSingleFrameClip("Fall", fall, false);
        AnimationClip duckClip = CreateSingleFrameClip("Duck", duck, true);
        AnimationClip hurtClip = CreateSingleFrameClip("Hurt", hurt, false);

        // ۳) ساخت AnimatorController
        string controllerPath = OutputFolder + "AriaShadow.controller";
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        controller.AddParameter("Speed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Grounded", AnimatorControllerParameterType.Bool);
        controller.AddParameter("VSpeed", AnimatorControllerParameterType.Float);
        controller.AddParameter("Ducking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("Hurt", AnimatorControllerParameterType.Trigger);

        AnimatorStateMachine sm = controller.layers[0].stateMachine;

        AnimatorState idleState = sm.AddState("Idle", new Vector3(250, 0, 0));
        idleState.motion = idleClip;
        AnimatorState walkState = sm.AddState("Walk", new Vector3(250, 100, 0));
        walkState.motion = walkClip;
        AnimatorState jumpState = sm.AddState("Jump", new Vector3(450, 0, 0));
        jumpState.motion = jumpClip;
        AnimatorState fallState = sm.AddState("Fall", new Vector3(450, 100, 0));
        fallState.motion = fallClip;
        AnimatorState duckState = sm.AddState("Duck", new Vector3(50, 100, 0));
        duckState.motion = duckClip;
        AnimatorState hurtState = sm.AddState("Hurt", new Vector3(250, -150, 0));
        hurtState.motion = hurtClip;

        sm.defaultState = idleState;

        // Idle <-> Walk
        AddTransition(idleState, walkState, ("Speed", true, 0.1f), ("Grounded", null, 0f));
        AddTransition(walkState, idleState, ("Speed", false, 0.1f), ("Grounded", null, 0f));

        // Idle/Walk -> Duck  و برگشت
        AddBoolTransition(idleState, duckState, "Ducking", true);
        AddBoolTransition(walkState, duckState, "Ducking", true);
        AddBoolTransition(duckState, idleState, "Ducking", false);

        // پرش و افتادن (از حالت‌های زمینی)
        AddGroundedFalseTransition(idleState, jumpState);
        AddGroundedFalseTransition(walkState, jumpState);
        AddGroundedFalseTransition(duckState, jumpState);

        AnimatorStateTransition jumpToFall = jumpState.AddTransition(fallState);
        jumpToFall.hasExitTime = false;
        jumpToFall.duration = 0.05f;
        jumpToFall.AddCondition(AnimatorConditionMode.Less, 0.1f, "VSpeed");

        AnimatorStateTransition fallToIdle = fallState.AddTransition(idleState);
        fallToIdle.hasExitTime = false;
        fallToIdle.duration = 0.05f;
        fallToIdle.AddCondition(AnimatorConditionMode.If, 0, "Grounded");

        // ضربه خوردن (Hurt) - از هر حالتی قابل فعال شدنه
        AnimatorStateTransition anyToHurt = sm.AddAnyStateTransition(hurtState);
        anyToHurt.hasExitTime = false;
        anyToHurt.duration = 0f;
        anyToHurt.AddCondition(AnimatorConditionMode.If, 0, "Hurt");

        AnimatorStateTransition hurtToIdle = hurtState.AddTransition(idleState);
        hurtToIdle.hasExitTime = true;
        hurtToIdle.exitTime = 0.8f;
        hurtToIdle.duration = 0.1f;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("<color=green>AnimatorController با موفقیت ساخته شد: " + controllerPath + "</color>\nحالا فقط کافیه این Controller رو به کامپوننت Animator روی Aria و Shadow درگ کنی.");
    }

    private static Sprite LoadSprite(string name)
    {
        return AssetDatabase.LoadAssetAtPath<Sprite>(SpriteFolder + name + ".png");
    }

    private static AnimationClip CreateSingleFrameClip(string name, Sprite sprite, bool loop)
    {
        AnimationClip clip = new AnimationClip { frameRate = 1 };
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = "",
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[1];
        keys[0] = new ObjectReferenceKeyframe { time = 0f, value = sprite };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        AssetDatabase.CreateAsset(clip, OutputFolder + name + ".anim");
        return clip;
    }

    private static AnimationClip CreateMultiFrameClip(string name, Sprite[] sprites, float fps, bool loop)
    {
        AnimationClip clip = new AnimationClip { frameRate = fps };
        var settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        EditorCurveBinding binding = new EditorCurveBinding
        {
            path = "",
            type = typeof(SpriteRenderer),
            propertyName = "m_Sprite"
        };
        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[sprites.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = sprites[i] };
        }
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        AssetDatabase.CreateAsset(clip, OutputFolder + name + ".anim");
        return clip;
    }

    // انتقال با شرط Speed (بالاتر/پایین‌تر از حد آستانه) + Grounded == true
    private static void AddTransition(AnimatorState from, AnimatorState to, (string param, bool? greater, float threshold) speedCond, (string param, object unused, float unused2) groundedCond)
    {
        AnimatorStateTransition t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0.05f;
        t.AddCondition(speedCond.greater == true ? AnimatorConditionMode.Greater : AnimatorConditionMode.Less, speedCond.threshold, speedCond.param);
        t.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
    }

    private static void AddBoolTransition(AnimatorState from, AnimatorState to, string param, bool value)
    {
        AnimatorStateTransition t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0.05f;
        t.AddCondition(value ? AnimatorConditionMode.If : AnimatorConditionMode.IfNot, 0, param);
        t.AddCondition(AnimatorConditionMode.If, 0, "Grounded");
    }

    private static void AddGroundedFalseTransition(AnimatorState from, AnimatorState to)
    {
        AnimatorStateTransition t = from.AddTransition(to);
        t.hasExitTime = false;
        t.duration = 0.05f;
        t.AddCondition(AnimatorConditionMode.IfNot, 0, "Grounded");
    }
}
