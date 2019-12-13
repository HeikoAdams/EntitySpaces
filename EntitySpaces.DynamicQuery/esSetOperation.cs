﻿/*  New BSD License
-------------------------------------------------------------------------------
Copyright (c) 2006-2012, EntitySpaces, LLC
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the EntitySpaces, LLC nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL EntitySpaces, LLC BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
-------------------------------------------------------------------------------
*/

using EntitySpaces.Interfaces;
using System;
using System.Runtime.Serialization;

namespace EntitySpaces.DynamicQuery
{
    /// <summary>
    /// Created when Query.Union (UnionAll, Intersect, Except) is called.
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "es", IsReference = true)]
    public class esSetOperation
    {
        /// <summary>
        /// The Constructor
        /// </summary>
        protected esSetOperation() { }

        /// <summary>
        /// The Constructor
        /// </summary>
        public esSetOperation(esDynamicQuery query)
        {
            this.Query = query;
        }

        /// <summary>
        /// The Query to form the Set with
        /// </summary>
        [DataMember(Name = "Query", EmitDefaultValue = false)]
        public esDynamicQuery Query;
        /// <summary>
        /// The Set Type, Union/Unionall/Intersect/Except
        /// </summary>
        [DataMember(Name = "Type", EmitDefaultValue = false)]
        public esSetOperationType SetOperationType;
    }
}
