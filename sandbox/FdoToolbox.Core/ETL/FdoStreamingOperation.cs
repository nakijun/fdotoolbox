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
    /// A base operation class that processes features sent to it in a streaming 
    /// fashion. (ie. operations further down the pipeline will get features sent to
    /// it as they are processed, instead of waiting for all features to be processed)
    /// </summary>
    public abstract class FdoStreamingOperation : IFdoOperation
    {
        public virtual IEnumerable<FdoFeature> Process(IEnumerable<FdoFeature> features)
        {
            foreach (FdoFeature feat in features)
            {
                ProcessFeature(feat);
                yield return feat;
            }
        }

        /// <summary>
        /// Processes a feature
        /// </summary>
        /// <param name="feat">The feature to process</param>
        /// <returns>true to pass this feature down the pipeline, false to discard this feature</returns>
        protected abstract void ProcessFeature(FdoFeature feat);
    }
}
