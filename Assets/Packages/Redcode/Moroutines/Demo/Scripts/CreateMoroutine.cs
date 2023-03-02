using System.Collections;
using UnityEngine;

namespace Redcode.Moroutines.Demo
{
    public class CreateMoroutine : MonoBehaviour
    {
        private void Start() => Moroutine.Run(TickEnumerable());

        private IEnumerator TickEnumerable()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                print("Tick!");
            }
        }



    }
}