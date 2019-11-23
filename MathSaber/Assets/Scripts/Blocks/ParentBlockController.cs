using System.Collections.Generic;
using UnityEngine;

namespace Blocks
{
    public class ParentBlockController : MonoBehaviour
    {
        private readonly List<EquationBlockController> _equationBlockControllers = new List<EquationBlockController>();

        #region External Functions

        public void AddEquationBlock(EquationBlockController equationBlockController)
        {
            equationBlockController.OnBlockDestroyed += HandleChildDestroyed;
            _equationBlockControllers.Add(equationBlockController);
        }

        public void NotifyParentCollision()
        {
            foreach (EquationBlockController equationBlockController in _equationBlockControllers)
            {
                equationBlockController.SetParentCollided();
            }
        }

        public void MakeAllBlocksFall()
        {
            foreach (EquationBlockController equationBlockController in _equationBlockControllers)
            {
                equationBlockController.FallBlock();
            }
        }

        public void MakeAllBlocksFlashFall()
        {
            foreach (EquationBlockController equationBlockController in _equationBlockControllers)
            {
                equationBlockController.FallFlashBlock();
            }
        }

        #endregion

        #region Utility Functions

        private void HandleChildDestroyed(EquationBlockController equationBlockController)
        {
            equationBlockController.OnBlockDestroyed -= HandleChildDestroyed;
            _equationBlockControllers.Remove(equationBlockController);

            if (_equationBlockControllers.Count <= 0)
            {
                Destroy(gameObject);
            }
        }

        #endregion
    }
}