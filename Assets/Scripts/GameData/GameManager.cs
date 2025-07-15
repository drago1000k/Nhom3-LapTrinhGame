using UnityEngine;

public class GameManager : MonoBehaviour
{
    private string selectedClass;

    void Start()
    {
        // Đọc class từ PlayerPrefs
        selectedClass = PlayerPrefs.GetString("SelectedClass", "DefaultClass");
        Debug.Log("Playing as: " + selectedClass);

        // Áp dụng logic dựa trên class
        ApplyClass(selectedClass);
    }

    void ApplyClass(string className)
    {
        switch (className)
        {
            case "BLADE ALPHA":
                Debug.Log("Applied Class BLADE ALPHA");
                // Ví dụ: Gán stats cho nhân vật
                break;
            case "TECH GAMA":
                Debug.Log("Applied Class TECH GAMA");
                // Ví dụ: Gán stats khác
                break;
            default:
                Debug.LogWarning("Unknown class: " + className);
                break;
        }
    }
}