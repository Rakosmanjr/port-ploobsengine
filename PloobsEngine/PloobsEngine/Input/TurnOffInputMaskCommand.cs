﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PloobsEngine.Commands;

namespace PloobsEngine.Input
{
    /// <summary>
    /// Turn n Input Mask OFF
    /// </summary>
    public class TurnOffInputMaskCommand : ICommand
    {
        InputMask mask;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurnOffInputMaskCommand"/> class.
        /// </summary>
        /// <param name="mask">The mask.</param>
        public TurnOffInputMaskCommand(InputMask mask)
        {
            this.mask = mask;
        }

        #region ICommand Members

        private InputAdvanced ia;

        /// <summary>
        /// Executes the command Call.
        /// </summary>
        protected override void execute()
        {
            ia.TurnOffInputMask(mask);
        }

        /// <summary>
        /// Sets the command target.
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected override void setTarget(object obj)
        {
            ia = (InputAdvanced)obj;
        }
        #endregion

        #region ICommand Members


        /// <summary>
        /// Gets the name of the command target.
        /// </summary>
        /// <value>
        /// The name of the target.
        /// </value>
        public override string TargetName
        {
            get { return InputAdvanced.MyName; }
        }
        #endregion


    }
}
