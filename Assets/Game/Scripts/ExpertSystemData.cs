using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
[CreateAssetMenu(fileName = "New Expert System", menuName = "FuzzyLogic/System data", order = 3)]
public class ExpertSystemData : ScriptableObject
{

    public FuzzyVariableInfo[] inputVariables = {};

    public FuzzyVariableInfo[] outputVariables= {};

    public FuzzyRuleInfo[] rules = {};

   // public bool[] showInputTerms = {};
    //public bool[] showOutputTerms = {};
}

[Serializable]
public class FuzzyVariableInfo
{
    public string name;

    public FuzzyTerm[] possibleTerms = {};

    public FuzzyVariableInfo GetCopy()
    {
        var n = new FuzzyVariableInfo();
        n.name = name;
        var terms = new FuzzyTerm[possibleTerms.Length];
        for (int i = 0; i < possibleTerms.Length; i++)
        {
            terms[i] = possibleTerms[i].GetCopy();
        }

        n.possibleTerms = terms;

        return n;
    }
}
[Serializable]
public class FuzzyTerm
{
    public string name;
    public float lowerBound;
    public float upperBound;

    public FuzzyTerm GetCopy()
    {
        var n = new FuzzyTerm();
        n.name = name;
        n.lowerBound = lowerBound;
        n.upperBound = upperBound;

        return n;
    }
}

[Serializable]
public class FuzzyRuleInfo
{
    public FuzzyStatement[] conditions = {};
    public FuzzyStatement[] conclusions = {};
}
[Serializable]
public class FuzzyStatement
{
    public FuzzyVariableInfo variable;
    public string term;
}

[CustomEditor(typeof(ExpertSystemData))]
public class ExpertSystemDataEditor : Editor
{
    private ExpertSystemData data;

    private List<List<ReorderableList>> conditionLists;
    private List<List<ReorderableList>> conclusionLists;

    private ReorderableList rulesList;
    private void OnEnable()
    {
        data = target as ExpertSystemData;
        conditionLists = new List<List<ReorderableList>>();
        conclusionLists = new List<List<ReorderableList>>();

        for (int i = 0; i < data.rules.Length; i++)
        {
            conditionLists.Add(new List<ReorderableList>());
            conclusionLists.Add(new List<ReorderableList>());
            ///Conditions
            SerializedProperty conditions = serializedObject.FindProperty("rules").GetArrayElementAtIndex(i)
                .FindPropertyRelative("conditions");
            
            var conditionList = new ReorderableList(serializedObject, conditions, true, true, true, true);

            conditionList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>

            {
                SerializedProperty conditionElement = conditions.GetArrayElementAtIndex(j);

                string[] variableNames = data.inputVariables.Select(x => x.name).ToArray();
                int variableIndex = Array.IndexOf(variableNames,
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue);

                rect.y += 1;
                variableIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    $"Condition {j + 1} Variable",
                    variableIndex,
                    variableNames);
                
                if (variableIndex >= 0)
                {
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue =
                        data.inputVariables[variableIndex].name;
                }
                else
                {
                    conditionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue = null;
                }

                if (variableIndex >= 0)
                {
                    FuzzyVariableInfo variable = data.inputVariables[variableIndex];
                    string[] terms = variable.possibleTerms.Select(x=>x.name).ToArray();
                    int termIndex = Array.IndexOf(terms, conditionElement.FindPropertyRelative("term").stringValue);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    termIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        $"Condition {j + 1} Term",
                        termIndex,
                        terms);
                    if (termIndex >= 0 && termIndex < terms.Length)
                    {
                        conditionElement.FindPropertyRelative("term").stringValue = terms[termIndex];
                    }
                }
            };
            conditionList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Conditions"); };
            conditionList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                //element.stringValue = "New Term";
            };
            conditionList.elementHeightCallback = index =>
            {
                var element = conditions.GetArrayElementAtIndex(index);

                var height = EditorGUIUtility.singleLineHeight * 2;

                return height;
            };
            
            conditionLists[i].Add(conditionList);
            
            
            ///Conclusions
            SerializedProperty conclusions = serializedObject.FindProperty("rules").GetArrayElementAtIndex(i)
                .FindPropertyRelative("conclusions");
            
            var conclusionList = new ReorderableList(serializedObject, conclusions, true, true, true, true);

            conclusionList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>

            {
                SerializedProperty conclusionElement = conclusions.GetArrayElementAtIndex(j);

                string[] variableNames = data.outputVariables.Select(x => x.name).ToArray();
                int variableIndex = Array.IndexOf(variableNames,
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue);

                rect.y += 1;
                variableIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    $"conclusion {j + 1} Variable",
                    variableIndex,
                    variableNames);
                
                if (variableIndex >= 0)
                {
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue =
                        data.outputVariables[variableIndex].name;
                }
                else
                {
                    conclusionElement.FindPropertyRelative("variable").FindPropertyRelative("name").stringValue = null;
                }

                if (variableIndex >= 0)
                {
                    FuzzyVariableInfo variable = data.outputVariables[variableIndex];
                    string[] terms = variable.possibleTerms.Select(x=>x.name).ToArray();
                    int termIndex = Array.IndexOf(terms, conclusionElement.FindPropertyRelative("term").stringValue);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                    termIndex = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                        $"conclusion {j + 1} Term",
                        termIndex,
                        terms);
                    if (termIndex >= 0 && termIndex < terms.Length)
                    {
                        conclusionElement.FindPropertyRelative("term").stringValue = terms[termIndex];
                    }
                }
            };
            conclusionList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "conclusions"); };
            conclusionList.onAddCallback = (ReorderableList list) =>
            {
                int index = list.serializedProperty.arraySize;
                list.serializedProperty.arraySize++;
                list.index = index;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index);
                //element.stringValue = "New Term";
            };
            conclusionList.elementHeightCallback = index =>
            {
                var height = EditorGUIUtility.singleLineHeight * 2;

                return height;
            };
            
            conclusionLists[i].Add(conclusionList);
        }
        
        SerializedProperty rules = serializedObject.FindProperty("rules");

        rulesList = new ReorderableList(serializedObject, rules, true, true, true, true);

        rulesList.drawElementCallback = (Rect rect, int j, bool isActive, bool isFocused) =>
        {
            EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width,EditorGUIUtility.singleLineHeight),$"Rule: {j}");
            rect.y += EditorGUIUtility.singleLineHeight;
            for (int i = 0; i < conditionLists[j].Count; i++)
            {
                conditionLists[j][i].DoList(new Rect(rect.x,rect.y,rect.width,rect.height-EditorGUIUtility.singleLineHeight));
                rect.y += conditionLists[j][i].GetHeight();
                conclusionLists[j][i].DoList(new Rect(rect.x,rect.y,rect.width,rect.height-EditorGUIUtility.singleLineHeight));
            }
            
        };
        
        rulesList.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Rules"); };
        
        rulesList.elementHeightCallback = index =>
        {
            float height = EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < conditionLists[index].Count; i++)
            {
                height+=conditionLists[index][i].GetHeight();
                height+=conclusionLists[index][i].GetHeight();
            }
           
            return height;
        };
    }
    
    
    
    

    public override void OnInspectorGUI()
    {
        DrawScriptField();
        
        serializedObject.Update();

        // Input variables
        EditorGUILayout.PropertyField(serializedObject.FindProperty("inputVariables"), true);
        EditorGUILayout.Space();
        //Output variables
        EditorGUILayout.PropertyField(serializedObject.FindProperty("outputVariables"), true);

        // Rules
        EditorGUILayout.Space();
        
        EditorGUI.BeginChangeCheck();
        rulesList.DoLayoutList();
        
        // Add buttons to add new input/output variables and rules
        if (GUILayout.Button("Add Input Variable"))
        {
            Array.Resize(ref data.inputVariables, data.inputVariables.Length + 1);
            data.inputVariables[data.inputVariables.Length - 1] = new FuzzyVariableInfo();
        }

        if (GUILayout.Button("Add Output Variable"))
        {
            Array.Resize(ref data.outputVariables, data.outputVariables.Length + 1);
            data.outputVariables[data.outputVariables.Length - 1] = new FuzzyVariableInfo();
        }

        if (GUILayout.Button("Add Rule"))
        {
            Array.Resize(ref data.rules, data.rules.Length + 1);
            data.rules[data.rules.Length - 1] = new FuzzyRuleInfo();
        }

        serializedObject.ApplyModifiedProperties();

        foreach (var rule in data.rules)
        {
            foreach (var cond in rule.conditions)
            {
                cond.variable =
                    data.inputVariables[
                        Array.FindIndex(data.inputVariables,(v)=>v.name==cond.variable.name)
                    ].GetCopy();
            }

            foreach (var conclusion in rule.conclusions)
            {
                conclusion.variable =
                    data.outputVariables[
                        Array.FindIndex(data.outputVariables,(v)=>v.name==conclusion.variable.name)
                    ].GetCopy();
            }
        }
    }
    
    private void DrawScriptField()
    {
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ExpertSystemData)target), typeof(ExpertSystemData), false);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
    }
}
