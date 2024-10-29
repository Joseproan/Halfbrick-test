using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartJump : MonoBehaviour
{
    public PressurePad trampoline;
    public void TrampolineJump()
    {
        trampoline.startJump = true;
    }
}
