using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using JDict = System.Collections.Generic.Dictionary<string,object>;
using JList = System.Collections.Generic.List<object>;

public static class RXJson
{
	public static string Encode(JDict rootDict)
	{
		StringBuilder sb = new StringBuilder();

		InnerEncode(sb, rootDict);

		return sb.ToString();
	}

	static void InnerEncode(StringBuilder sb, object theObject)
	{
		if(theObject is string)
		{
			sb.AppendFormat("\"{0}\"",theObject);
		}
		else if(theObject is int)
		{
			sb.AppendFormat("\"{0}\"",theObject);
		}
		else if(theObject is float)
		{
			sb.AppendFormat("\"{0:0.000}\"",theObject);
		}
		else if(theObject is bool)
		{
			sb.AppendFormat("\"{0}\"",((bool)theObject) ? "true" : "false");
		}
		else if(theObject is JDict)
		{
			JDict theDict = (theObject as JDict);
			int count = theDict.Count;

			int i = 0;

			sb.Append("{");

			foreach(var keyval in theDict)
			{
				string key = keyval.Key;
				object val = keyval.Value;

				sb.Append("\""+key+"\":");

				InnerEncode(sb,val);

				if(i < count-1) //don't put a comma on the last item
				{
					sb.Append(",");
				}

				i++;
			}
			sb.Append("}");
		}
		else if(theObject is JList)
		{
			JList theList = (theObject as JList);

			int count = theList.Count;
			
			sb.Append("[");
			
			for(int t = 0; t<count; t++)
			{
				InnerEncode(sb,theList[t]);
				
				if(t < count-1) //don't put a comma on the last item
				{
					sb.Append(",");
				}
			}

			sb.Append("]");
		}
	}
}










