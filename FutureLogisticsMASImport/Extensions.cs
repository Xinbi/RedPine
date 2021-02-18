// Decompiled with JetBrains decompiler
// Type: FutureLogisticsMASImport.Extensions
// Assembly: FutureLogisticsMASImport, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6C3B527D-500A-4CC6-B04D-583364735731
// Assembly location: C:\Temp\Red Pine Systems\FutureLogisticsMASImport.exe

using System;
using System.Linq;
using System.Xml.Linq;

namespace FutureLogisticsMASImport
{
  public static class Extensions
  {
    public static T GetAttributeValue<T>(
      this XElement elem,
      string attributeName,
      params T[] defaultValue)
    {
      return elem._GetAttributeValue<T>(attributeName, StringComparison.OrdinalIgnoreCase, defaultValue);
    }

    public static T GetAttributeValue<T>(
      this XElement elem,
      string attributeName,
      StringComparison comparison,
      params T[] defaultValue)
    {
      return elem._GetAttributeValue<T>(attributeName, comparison, defaultValue);
    }

    public static T GetElementValue<T>(this XElement elem, params T[] defaultValue)
    {
      T obj = defaultValue.Length > 0 ? defaultValue[0] : default (T);
      if (elem.Value != null)
      {
        try
        {
          Type type = Nullable.GetUnderlyingType(typeof (T));
          if ((object) type == null)
            type = typeof (T);
          Type conversionType = type;
          obj = (T) Convert.ChangeType((object) elem.Value, conversionType);
        }
        catch (InvalidCastException ex)
        {
        }
        catch (FormatException ex)
        {
        }
        catch
        {
          throw;
        }
      }
      return obj;
    }

    public static bool HasAttribute(this XElement elem, string attributeName)
    {
      return elem.Attributes().Count<XAttribute>((Func<XAttribute, bool>) (a => a.Name.ToString().Equals(attributeName, StringComparison.OrdinalIgnoreCase))) > 0;
    }

    public static bool HasAttribute(
      this XElement elem,
      string attributeName,
      StringComparison comparison)
    {
      return elem.Attributes().Count<XAttribute>((Func<XAttribute, bool>) (a => a.Name.ToString().Equals(attributeName, comparison))) > 0;
    }

    public static int? ToNullableInt(this object valueToConvert)
    {
      int? nullable = new int?();
      if (valueToConvert != null)
      {
        if (valueToConvert.GetType().IsEnum)
          return (int?) valueToConvert;
        int result;
        if (int.TryParse(valueToConvert.ToString(), out result))
          return new int?(result);
      }
      return nullable;
    }

    private static T _GetAttributeValue<T>(
      this XElement elem,
      string attributeName,
      StringComparison comparison,
      params T[] defaultValue)
    {
      T obj = defaultValue.Length > 0 ? defaultValue[0] : default (T);
      XAttribute xattribute = elem.Attributes().FirstOrDefault<XAttribute>((Func<XAttribute, bool>) (a => a.Name.ToString().Equals(attributeName, comparison)));
      if (xattribute != null)
      {
        try
        {
          Type type = Nullable.GetUnderlyingType(typeof (T));
          if ((object) type == null)
            type = typeof (T);
          Type conversionType = type;
          obj = (T) Convert.ChangeType((object) xattribute.Value, conversionType);
        }
        catch (InvalidCastException ex)
        {
        }
        catch (FormatException ex)
        {
        }
        catch
        {
          throw;
        }
      }
      return obj;
    }
  }
}
