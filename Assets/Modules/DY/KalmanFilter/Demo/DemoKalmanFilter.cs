using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


[ExecuteInEditMode]
public class DemoKalmanFilter : MonoBehaviour {

    Material mat;

    [SerializeField]
    bool m_Debug;

    [SerializeField]
    bool m_SideBySide;


    [SerializeField] [Range(0, 4)] public float Q_measurementNoise;// measurement noise

    [SerializeField] [Range(0, 100)] public float R_EnvironmentNoize; // environment noise
    [SerializeField] [Range(0, 100)] public float F_facorOfRealValueToPrevious; // factor of real value to previous real value
    [SerializeField] [Range(0.1f, 100)] public float H_factorOfMeasuredValue; // factor of measured value to real value
    [SerializeField] public float m_StartState;
    [SerializeField] public float m_Covariance;

    [SerializeField] int m_CountOfElements;
    [SerializeField] int m_VerticalSize;
    private int m_oldCount = 0;

    float[] Vertexex = null;
    float[] FilteredVertexes;

    private int currentNumberInStep = 0;
    private float m_currentStep;

    private void OnValidate() {
        if (m_oldCount != m_CountOfElements) {
            m_oldCount = m_CountOfElements;
            GenerateTestNumbers();
        }

    }

    private void Update() {
        GenerateNewNumbers();
    }

   
    void Start() {
        mat = new Material(Shader.Find("UI/Default"));
        TestFilter();
    }

    private void TestFilter() {
        FilteredVertexes = new float[m_CountOfElements];

        var filter = new Filters.KalmanFilter(RealValueToPreviousRealValue: F_facorOfRealValueToPrevious, MeasuredToRealValue: H_factorOfMeasuredValue, mesurementNoize: Q_measurementNoise, environmentNoize: R_EnvironmentNoize);
       
        // Setting starting values for State и Covariance
        filter.State = Vertexex[0];
        filter.Covariance = m_Covariance;

        //Applying filter for previously generated values
        for (int i = 0; i < m_CountOfElements; i++) {
            if (i == 0) {
                FilteredVertexes[i] = Vertexex[0];
            } else {
                filter.Correct(Vertexex[i]); // Applying Algorythm
                FilteredVertexes[i] = filter.State; // Saving Current State 
            }
        }

    }


    void OnPostRender() {
        if (m_Debug) {
            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.red);
            for (int i = 0; i < m_CountOfElements; i++) {
                float offset = (float)i / m_CountOfElements;
                if (m_SideBySide) {
                    offset = offset / 2;
                }
                GL.Vertex(new Vector3(offset, Vertexex[i] / m_VerticalSize, 0));
            }
            if (mat != null)
                mat.SetPass(0);
            GL.End();
            GL.PopMatrix();


            GL.PushMatrix();
            GL.LoadOrtho();
            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.green);

            for (int i = 0; i < m_CountOfElements; i++) {
                float offset = (float)i / m_CountOfElements;

                if (m_SideBySide) {
                    offset = (offset / 2) + 0.5f;
                }
                GL.Vertex(new Vector3(offset, FilteredVertexes[i] / m_VerticalSize, 0));
            }
            if (mat != null)
                mat.SetPass(0);
            GL.End();
            GL.PopMatrix();
        }
    }

    private void GenerateTestNumbers() {
        Vertexex = new float[m_CountOfElements];
        for (int step = 0; step < 5; step++) {

            float target = UnityEngine.Random.Range(0, 0.8f);
            for (int i = 0; i < m_CountOfElements / 5; i++) {
                Vertexex[i + (step * m_CountOfElements / 5)] = 0f + (target + UnityEngine.Random.value / 10);
            }
        }

    }

    private float ElementsInStep {
        get {
            return m_CountOfElements / 5;
        }
    }

    void GenerateNewNumbers() {
        if (currentNumberInStep == ElementsInStep) {
            currentNumberInStep = 0;
            m_currentStep = UnityEngine.Random.Range(0, 0.8f);
        } else {
            currentNumberInStep++;
        }

        InsertElement(m_currentStep + UnityEngine.Random.value / 5);
        TestFilter();
        OnPostRender();
    }

    void InsertElement(float value) {
        var newArray = new float[Vertexex.Length];
        Array.Copy(Vertexex, 1, newArray, 0, Vertexex.Length - 1);
        newArray[Vertexex.Length - 1] = value;
        //Debug.Log("New Element " + newArray[Vertexex.Length - 1]);
        Vertexex = newArray;
    }

}
