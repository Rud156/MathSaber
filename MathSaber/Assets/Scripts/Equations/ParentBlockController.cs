using System.Collections.Generic;
using General;
using UnityEngine;

namespace Equations
{
    public class ParentBlockController : MonoBehaviour
    {
        private readonly List<EquationBlockController> _equationBlockControllers = new List<EquationBlockController>();

        #region External Functions

        public void AddEquationBlock(EquationBlockController equationBlockController) => _equationBlockControllers.Add(equationBlockController);

        public void NotifyParentCollision()
        {
            foreach (EquationBlockController equationBlockController in _equationBlockControllers)
            {
                equationBlockController.SetParentCollided();
            }
        }

        #endregion
    }
}