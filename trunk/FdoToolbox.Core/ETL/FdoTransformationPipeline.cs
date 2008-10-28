#region LGPL Header
// Copyright (C) 2008, Jackie Ng
// http://code.google.com/p/fdotoolbox, jumpinjackie@gmail.com
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace FdoToolbox.Core.ETL
{
    /// <summary>
    /// Represents an ETL (Extract Transform Load) pipeline for feature data
    /// </summary>
    public class FdoTransformationPipeline
    {
        private List<IFdoOperation> _Operations;

        private IFdoInput _input;

        public IFdoInput Input
        {
            get { return _input; }
            set { _input = value; }
        }

        private IFdoOutput _output;

        public IFdoOutput Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public FdoTransformationPipeline() 
        {
            _Operations = new List<IFdoOperation>(); 
        }

        public FdoTransformationPipeline(IFdoInput input, IFdoOutput output)
            : this()
        {
            _input = input;
            _output = output;
        }

        /// <summary>
        /// Adds an operation to the pipeline. 
        /// </summary>
        /// <param name="filter">The filter to add</param>
        public void RegisterOperation(IFdoOperation filter)
        {
            _Operations.Add(filter);
        }

        /// <summary>
        /// Executes this transformation pipeline
        /// </summary>
        public void Execute()
        {
            //Pre-execution validation and setup
            if (_input == null)
                throw new InvalidOperationException("No input defined");

            if (_output == null)
                throw new InvalidOperationException("No output defined");

            //Begin execution
            IEnumerable<FdoFeature> features = new FdoFeature[0];
            features = _input.Process(features);
            foreach (IFdoOperation filter in _Operations)
            {
                features = filter.Process(features);
            }
            //Send to output
            features = _output.Process(features);
            //Loop and dispose each feature that reaches the end of the pipeline
            IEnumerator<FdoFeature> enumerator = features.GetEnumerator();
            while (enumerator.MoveNext()) { enumerator.Current.Dispose(); };
        }
    }
}
