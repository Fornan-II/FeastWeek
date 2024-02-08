using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugITem : MonoBehaviour
{
    public static bool Allow = false;

    // Start is called before the first frame update
    void Start()
    {
        if(!GlobalData.HasCompletedGame)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Allow = true;
        MsgBox inst = MsgBox.GetInstance(MsgBox.MsgBoxType.ToolTip);
        inst.ShowMessage("Press ` for debug", 7f);
        Destroy(gameObject);
    }
}
