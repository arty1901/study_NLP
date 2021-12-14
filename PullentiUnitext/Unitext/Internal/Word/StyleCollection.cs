/*
 * SDK Pullenti Unitext, version 4.10, november 2021. Copyright (c) 2013, Pullenti. All rights reserved. 
 * Non-Commercial Freeware and Commercial Software. 
 * This class is generated using the converter UniSharping (www.unisharping.ru) from Pullenti C# project. 
 * The latest version of the code is available on the site www.pullenti.ru
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Pullenti.Unitext.Internal.Word
{
    class StyleCollection
    {
        public const string PropertyNamePrefix = "sprm";
        IEnumerable<Prl[]> prls;
        public object this[string name]
        {
            get
            {
                return this.Get(name);
            }
        }
        internal StyleCollection(IEnumerable<Prl[]> prls)
        {
            this.prls = prls;
        }
        public override string ToString()
        {
            StringBuilder res = new StringBuilder();
            foreach (Prl[] prlSet in prls) 
            {
                foreach (Prl prl in prlSet) 
                {
                    string s = SinglePropertyModifiers.GetSprmName(prl.sprm.sprm);
                    if (s != null) 
                        res.AppendFormat("{0}; ", s);
                }
            }
            return res.ToString();
        }
        public object GetByUshort(ushort sprm)
        {
            foreach (Prl[] prlSet in prls) 
            {
                foreach (Prl prl in prlSet) 
                {
                    if (prl.sprm.sprm == sprm) 
                        return SinglePropertyValue.ParseValue(sprm, prl.operand);
                }
            }
            return null;
        }
        public object Get(string name)
        {
            if (name == null) 
                throw new ArgumentNullException("name");
            ushort sprm = SinglePropertyModifiers.GetSprmByName(name);
            foreach (Prl[] prlSet in prls) 
            {
                foreach (Prl prl in prlSet) 
                {
                    if (prl.sprm.sprm == sprm) 
                        return SinglePropertyValue.ParseValue(sprm, prl.operand);
                }
            }
            return null;
        }
        public IEnumerable<object> GetAll(string name)
        {
            if (name == null) 
                throw new ArgumentNullException("name");
            ushort sprm = SinglePropertyModifiers.GetSprmByName(name);
            List<object> res = new List<object>();
            foreach (Prl[] prlSet in prls) 
            {
                foreach (Prl prl in prlSet) 
                {
                    if (prl.sprm.sprm == sprm) 
                        res.Add(SinglePropertyValue.ParseValue(sprm, prl.operand));
                }
            }
            return res;
        }
        public IEnumerable<string> GetNames()
        {
            return null;
        }
    }
}