using System;
using System.Collections;

namespace OpenQA.Selenium.Remote
{
    public abstract class RemoteSetting
    {
        public bool IsValid
        {
            get { return this.IsJsonSerializable(this.GenerateSerializableObject()); }
        }

        public object SerializableObject
        {
            get { return this.GenerateSerializableObject(); }
        }

        protected abstract object GenerateSerializableObject();

        private bool IsJsonSerializable(object arg)
        {
            IEnumerable argAsEnumerable = arg as IEnumerable;
            IDictionary argAsDictionary = arg as IDictionary;

            if (arg is string || arg is float || arg is double || arg is int || arg is long || arg is bool || arg == null)
            {
                return true;
            }
            else if (argAsDictionary != null)
            {
                foreach (object key in argAsDictionary.Keys)
                {
                    if (!(key is string))
                    {
                        return false;
                    }
                }

                foreach (object value in argAsDictionary.Values)
                {
                    if (!IsJsonSerializable(value))
                    {
                        return false;
                    }
                }
            }
            else if (argAsEnumerable != null)
            {
                foreach (object item in argAsEnumerable)
                {
                    if (!IsJsonSerializable(item))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}