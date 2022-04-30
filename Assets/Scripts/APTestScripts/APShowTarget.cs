using UnityEngine;

[ExecuteInEditMode]
public class APShowTarget : MonoBehaviour
{
    #region Methods
    private void SetSize()
    {
        float scaleWidth = 10.0f * Screen.width / Screen.height;

        transform.localScale = new Vector3(scaleWidth, transform.localScale.y, 1);
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        SetSize();
    }
}
