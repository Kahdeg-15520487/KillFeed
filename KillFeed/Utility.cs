//using System;
//using System.ComponentModel;
//using System.Reflection;
//using System.Text;

//namespace KillFeed
//{
//    static class Utility
//    {
//        public static string DisplayObjectInfo(this Object o)
//        {
//            StringBuilder sb = new StringBuilder();

//            // Include the type of the object
//            Type type = o.GetType();
//            sb.AppendLine("Type: " + type.Name);

//            // Include information for each Field
//            sb.AppendLine("Fields:");

//            FieldInfo[] fi = type.GetFields();
//            if (fi != null)
//            {
//                if (fi.Length > 0)
//                {
//                    foreach (FieldInfo f in fi)
//                    {
//                        sb.AppendLine(f.ToString() + " = " + f.GetValue(o));
//                    }
//                }
//                else
//                {
//                    sb.AppendLine("None");
//                }
//            }

//            //sb.AppendLine();
//            //sb.AppendLine("Properties:");
//            //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(o))
//            //{
//            //    string name = descriptor.Name;
//            //    object value = descriptor.GetValue(o);
//            //    sb.AppendLine($"{name}={value}");
//            //}

//            // Include information for each Property
//            //sb.Append("\r\n\r\nProperties:");

//            //PropertyInfo[] pi = type.GetProperties();
//            //if (pi != null)
//            //{
//            //    if (pi.Length > 0)
//            //    {
//            //        foreach (PropertyInfo p in pi)
//            //        {
//            //            sb.Append("\r\n " + p.ToString() + " = " +
//            //                      p.GetValue(o, null));
//            //        }
//            //    }
//            //    else
//            //    {
//            //        sb.Append("\r\n None");
//            //    }
//            //}

//            return sb.ToString();
//        }
//    }
//}
