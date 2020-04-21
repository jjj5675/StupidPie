using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerCharacter))]
public class PlayerCharacterEditor : Editor
{
    SerializedProperty m_SpriteRendererProp;
    SerializedProperty m_DamageableProp;
    SerializedProperty m_DashableProp;

    SerializedProperty m_MoveSpeedProp;
    SerializedProperty m_GroundAccelerationProp;
    SerializedProperty m_GroundDecelerationProp;

    SerializedProperty m_DashDistanceProp;
    SerializedProperty m_TimeToDashPointProp;

    SerializedProperty m_JumpHeightProp;
    SerializedProperty m_TimeToJumpApexProp;

    SerializedProperty m_SlidingSpeedProp;
    SerializedProperty m_SlidingByWaitTimeProp;
    SerializedProperty m_MaxSlidingSpeedProp;
    SerializedProperty m_SlidingSpeedDecelProportionProp;
    SerializedProperty m_WallLeapVelocityProp;

    bool m_ReferencesFoldout;
    bool m_MovementSettingsFoldout;
    bool m_DashSettingsFoldout;
    bool m_AirborneSettingsFoldout;
    bool m_SlidingSettingsFoldout;

    readonly GUIContent m_SpriteRendererContent = new GUIContent("Sprite Renderer");
    readonly GUIContent m_DamageableContent = new GUIContent("Damageable");
    readonly GUIContent m_DashableContent = new GUIContent("Dashable");

    readonly GUIContent m_MoveSpeedContent = new GUIContent("Move Speed");
    readonly GUIContent m_GroundAccelerationContent = new GUIContent("Ground Acceleration");
    readonly GUIContent m_GroundDecelerationContent = new GUIContent("Ground Deceleration");

    readonly GUIContent m_DashDistanceContent = new GUIContent("Dash Distance");
    readonly GUIContent m_TimeToDashPointContent = new GUIContent("Time To Dash Point");

    readonly GUIContent m_JumpHeightContent = new GUIContent("Jump Height");
    readonly GUIContent m_TimeToJumpApexContent = new GUIContent("Time To Jump Apex");

    readonly GUIContent m_SlidingSpeedContent = new GUIContent("Sliding Speed");
    readonly GUIContent m_SlidingByWaitTimeContent = new GUIContent("Sliding By Wait Time");
    readonly GUIContent m_MaxSlidingSpeedContent = new GUIContent("Max Sliding Speed");
    readonly GUIContent m_SlidingSpeedDecelProportionContent = new GUIContent("Sliding Speed Decel Proportion");
    readonly GUIContent m_WallLeapVelocityContent = new GUIContent("Wall Leap Velocity");

    readonly GUIContent m_ReferencesContent = new GUIContent("References");
    readonly GUIContent m_MovementSettingsContent = new GUIContent("Movement Settings");
    readonly GUIContent m_DashSettingsContent = new GUIContent("Dash Settings");
    readonly GUIContent m_AirborneSettingsContent = new GUIContent("Airborne Settings");
    readonly GUIContent m_SlidingSettingsContent = new GUIContent("Sliding Settings");

    void OnEnable()
    {
        m_SpriteRendererProp = serializedObject.FindProperty("spriteRenderer");
        m_DamageableProp = serializedObject.FindProperty("damageable");
        m_DashableProp = serializedObject.FindProperty("dashable");

        m_MoveSpeedProp = serializedObject.FindProperty("moveSpeed");
        m_GroundAccelerationProp = serializedObject.FindProperty("groundAcceleration");
        m_GroundDecelerationProp = serializedObject.FindProperty("groundDeceleration");

        m_DashDistanceProp = serializedObject.FindProperty("dashDistance");
        m_TimeToDashPointProp = serializedObject.FindProperty("timeToDashPoint");

        m_JumpHeightProp = serializedObject.FindProperty("jumpHeight");
        m_TimeToJumpApexProp = serializedObject.FindProperty("timeToJumpApex");

        m_SlidingSpeedProp = serializedObject.FindProperty("slidingSpeed");
        m_SlidingByWaitTimeProp = serializedObject.FindProperty("slidingByWaitTime");
        m_MaxSlidingSpeedProp = serializedObject.FindProperty("maxSlidingSpeed");
        m_SlidingSpeedDecelProportionProp = serializedObject.FindProperty("slidingSpeedDecelProportion");
        m_WallLeapVelocityProp = serializedObject.FindProperty("wallLeapVelocity");

    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawReferencesGUI();
        DrawMovementSettingsGUI();
        DrawDashSettingsGUI();
        DrawAirborneSettingsGUI();
        DrawSlidingSettingsGUI();

        serializedObject.ApplyModifiedProperties();
    }

    public void DrawReferencesGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        m_ReferencesFoldout = EditorGUILayout.Foldout(m_ReferencesFoldout, m_ReferencesContent);

        if (m_ReferencesFoldout)
        {
            EditorGUILayout.PropertyField(m_SpriteRendererProp, m_SpriteRendererContent);
            EditorGUILayout.PropertyField(m_DamageableProp, m_DamageableContent);
            EditorGUILayout.PropertyField(m_DashableProp, m_DashableContent);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    public void DrawMovementSettingsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        m_MovementSettingsFoldout = EditorGUILayout.Foldout(m_MovementSettingsFoldout, m_MovementSettingsContent);

        if (m_MovementSettingsFoldout)
        {
            EditorGUILayout.PropertyField(m_MoveSpeedProp, m_MoveSpeedContent);
            EditorGUILayout.PropertyField(m_GroundAccelerationProp, m_GroundAccelerationContent);
            EditorGUILayout.PropertyField(m_GroundDecelerationProp, m_GroundDecelerationContent);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    public void DrawDashSettingsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        m_DashSettingsFoldout = EditorGUILayout.Foldout(m_DashSettingsFoldout, m_DashSettingsContent);

        if (m_DashSettingsFoldout)
        {
            EditorGUILayout.PropertyField(m_DashDistanceProp, m_DashDistanceContent);
            EditorGUILayout.PropertyField(m_TimeToDashPointProp, m_TimeToDashPointContent);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    public void DrawAirborneSettingsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        m_AirborneSettingsFoldout = EditorGUILayout.Foldout(m_AirborneSettingsFoldout, m_AirborneSettingsContent);

        if (m_AirborneSettingsFoldout)
        {
            EditorGUILayout.PropertyField(m_JumpHeightProp, m_JumpHeightContent);
            EditorGUILayout.PropertyField(m_TimeToJumpApexProp, m_TimeToJumpApexContent);
        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    public void DrawSlidingSettingsGUI()
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUI.indentLevel++;

        m_SlidingSettingsFoldout = EditorGUILayout.Foldout(m_SlidingSettingsFoldout, m_SlidingSettingsContent);

        if(m_SlidingSettingsFoldout)
        {
            EditorGUILayout.PropertyField(m_SlidingSpeedProp, m_SlidingSpeedContent);
            EditorGUILayout.PropertyField(m_MaxSlidingSpeedProp, m_MaxSlidingSpeedContent);
            EditorGUILayout.PropertyField(m_SlidingByWaitTimeProp, m_SlidingByWaitTimeContent);
            EditorGUILayout.PropertyField(m_SlidingSpeedDecelProportionProp, m_SlidingSpeedDecelProportionContent);
            EditorGUILayout.PropertyField(m_WallLeapVelocityProp, m_WallLeapVelocityContent);

        }

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }
}
