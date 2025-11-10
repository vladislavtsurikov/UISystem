//-----------------------------------------------------------------------
// <copyright file="DoubleLookupDictionary.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace OdinSerializer.Utilities
{
    /// <summary>
    ///     Not yet documented.
    /// </summary>
    [Serializable]
    public class
        DoubleLookupDictionary<TFirstKey, TSecondKey, TValue> : Dictionary<TFirstKey, Dictionary<TSecondKey, TValue>>
    {
        private readonly IEqualityComparer<TSecondKey> secondKeyComparer;

        public DoubleLookupDictionary() => secondKeyComparer = EqualityComparer<TSecondKey>.Default;

        public DoubleLookupDictionary(IEqualityComparer<TFirstKey> firstKeyComparer,
            IEqualityComparer<TSecondKey> secondKeyComparer)
            : base(firstKeyComparer) =>
            this.secondKeyComparer = secondKeyComparer;

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public new Dictionary<TSecondKey, TValue> this[TFirstKey firstKey]
        {
            get
            {
                Dictionary<TSecondKey, TValue> innerDict;

                if (!TryGetValue(firstKey, out innerDict))
                {
                    innerDict = new Dictionary<TSecondKey, TValue>(secondKeyComparer);
                    Add(firstKey, innerDict);
                }

                return innerDict;
            }
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public int InnerCount(TFirstKey firstKey)
        {
            Dictionary<TSecondKey, TValue> innerDict;

            if (TryGetValue(firstKey, out innerDict))
            {
                return innerDict.Count;
            }

            return 0;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public int TotalInnerCount()
        {
            var count = 0;

            if (Count > 0)
            {
                foreach (Dictionary<TSecondKey, TValue> innerDict in Values)
                {
                    count += innerDict.Count;
                }
            }

            return count;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public bool ContainsKeys(TFirstKey firstKey, TSecondKey secondKey)
        {
            Dictionary<TSecondKey, TValue> innerDict;

            return TryGetValue(firstKey, out innerDict) && innerDict.ContainsKey(secondKey);
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public bool TryGetInnerValue(TFirstKey firstKey, TSecondKey secondKey, out TValue value)
        {
            Dictionary<TSecondKey, TValue> innerDict;

            if (TryGetValue(firstKey, out innerDict) && innerDict.TryGetValue(secondKey, out value))
            {
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public TValue AddInner(TFirstKey firstKey, TSecondKey secondKey, TValue value)
        {
            if (ContainsKeys(firstKey, secondKey))
            {
                throw new ArgumentException("An element with the same keys already exists in the " +
                                            GetType().GetNiceName() + ".");
            }

            return this[firstKey][secondKey] = value;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public bool RemoveInner(TFirstKey firstKey, TSecondKey secondKey)
        {
            Dictionary<TSecondKey, TValue> innerDict;

            if (TryGetValue(firstKey, out innerDict))
            {
                var removed = innerDict.Remove(secondKey);

                if (innerDict.Count == 0)
                {
                    Remove(firstKey);
                }

                return removed;
            }

            return false;
        }

        /// <summary>
        ///     Not yet documented.
        /// </summary>
        public void RemoveWhere(Func<TValue, bool> predicate)
        {
            var toRemoveBufferFirstKey = new List<TFirstKey>();
            var toRemoveBufferSecondKey = new List<TSecondKey>();

            foreach (KeyValuePair<TFirstKey, Dictionary<TSecondKey, TValue>> outerDictionary in this.GFIterator())
            foreach (KeyValuePair<TSecondKey, TValue> innerKeyPair in outerDictionary.Value.GFIterator())
            {
                if (predicate(innerKeyPair.Value))
                {
                    toRemoveBufferFirstKey.Add(outerDictionary.Key);
                    toRemoveBufferSecondKey.Add(innerKeyPair.Key);
                }
            }

            for (var i = 0; i < toRemoveBufferFirstKey.Count; i++)
            {
                RemoveInner(toRemoveBufferFirstKey[i], toRemoveBufferSecondKey[i]);
            }
        }
    }
}
