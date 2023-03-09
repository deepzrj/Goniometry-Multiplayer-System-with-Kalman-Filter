using UnityEngine;
using Filters;

public class TestFilter : MonoBehaviour {

    private KalmanFilter m_Filter;

    void Start () {
        Debug.Log("Start Testing Filter");
        m_Filter = new KalmanFilter();
        m_Filter.State = 0; //Setting first (non filtered) value to 0 for example;
	}

    void Update () {
        Test();
    }

    void Test() {
        float AnyInputValue = Random.Range(-5f, 5f); //you can paste your values here

        float FilteredValue = m_Filter.FilterValue(AnyInputValue); //applying filter

        Debug.Log("TestingFilter: Dirty value = " + AnyInputValue + " Filtered value = " + FilteredValue); //printing output
    }
 
}
