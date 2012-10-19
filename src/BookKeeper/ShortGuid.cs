using System;
using System.Diagnostics.CodeAnalysis;

namespace BookKeeper
{
  /// <summary>
  /// A shorter 22 character base64 encoded version of a Guid, thats completely Guid compatible, particularly useful for urls.  Inspiration
  /// taken from:
  /// <a href="http://blog.muonlab.com/2010/03/24/shortening-guids-in-mvc-uris/">http://blog.muonlab.com/2010/03/24/shortening-guids-in-mvc-
  /// uris/</a>
  /// <a href="http://www.singular.co.nz/blog/archive/2007/12/20/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp.aspx">http:
  /// //www.singular.co.nz/blog/archive/2007/12/20/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp.aspx</a>
  /// </summary>
  /// <remarks>	Creating a new empty instance results in the equivalent to Guid.Empty.  The default constructor is fairly useless. </remarks>
  public struct ShortGuid : IEquatable<ShortGuid>
  {
    private const string EmptyEncoded = "AAAAAAAAAAAAAAAAAAAAAA";
    private readonly Guid _guid;
    private readonly string _value;

    /// <summary>	Creates a ShortGuid from a base64 encoded string OR any of the allowed Guid formats. </summary>
    /// <remarks>	ebrown, 6/25/2011. </remarks>
    /// <exception cref="ArgumentNullException">	Thrown when value is null. </exception>
    /// <exception cref="ArgumentException">		Thrown when value is whitespace. </exception>
    /// <exception cref="FormatException">		Thrown when value is not a properly formatted Guid string. </exception>
    /// <param name="value">	The encoded Guid as a base64 string OR any of the allowed Guid formats. </param>
    public ShortGuid(string value)
    {
      if (null == value) { throw new ArgumentNullException("value"); }
      if (string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("Value cannot be empty of whitespace", "value"); }

      _guid = Decode(value);
      _value = value.Length == 22 ? value : Encode(_guid);
    }

    /// <summary>	Creates a ShortGuid from an existing Guid. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <param name="value">	Unique identifier. </param>
    public ShortGuid(Guid value)
    {
      _guid = value;
      _value = Encode(value);
    }

    //this is only necessary because structs can't have parameterless constructors and we need our default Empty value to return the correct string value under those circumstances
    private string LazyResolvedValue()
    {
      return null == _value ? EmptyEncoded : _value;
    }

    /// <summary> The ShortGuid equivalent of Guid.Empty </summary>
    public static readonly ShortGuid Empty = new ShortGuid(Guid.Empty);

    /// <summary>	Convert this object into a shortened 22 character base64 string representation. </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <returns>	A string representation of this object. </returns>
    public override string ToString()
    {
      return LazyResolvedValue();
    }

    /// <summary>	Returns a value indicating whether this instance and a specified Object represent the same type and value. </summary>
    /// <remarks>	Will handle comparisons between ShortGuids, between Guid and ShortGuid and string and ShortGuid. </remarks>
    /// <param name="obj">	The object to compare. </param>
    /// <returns>	true if the objects are considered equal by underlying Guid or string values, false if they are not. </returns>
    public override bool Equals(object obj)
    {
      if (obj is ShortGuid) { return _guid.Equals(((ShortGuid)obj)._guid); }

      if (obj is Guid) { return _guid.Equals((Guid)obj); }

      if (obj is string) { return LazyResolvedValue().Equals(obj); }

      return false;
    }

    /// <summary>	Returns a value indicating whether this instance and a specified Object represent the same type and value. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <param name="other">	The short unique identifier to compare to this object. </param>
    /// <returns>	true if the objects are considered equal by underlying Guid values, false if they are not. </returns>
    public bool Equals(ShortGuid other)
    {
      return _guid.Equals(other._guid);
    }

    /// <summary>	Returns the hash code for this instance. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <returns>	A 32-bit signed integer that is the hash code for this instance. </returns>
    public override int GetHashCode()
    {
      return _guid.GetHashCode();
    }

    /// <summary>	Creates a new unique identifier. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <returns>	A new ShortGuid instance created from Guid.NewGuid. </returns>
    public static ShortGuid NewGuid()
    {
      return new ShortGuid(Guid.NewGuid());
    }

    private static string Encode(Guid guid)
    {
      return Convert.ToBase64String(guid.ToByteArray())
        .Substring(0, 22)
        .Replace('/', '_')
        .Replace('+', '-');
    }

    [SuppressMessage("Gendarme.Rules.BadPractice", "PreferTryParseRule", Justification = "Expectation is that the code will throw if the value is invaild")]
    private static Guid Decode(string value)
    {
      return value.Length == 22 ?
        new Guid(Convert.FromBase64String(value.Replace('_', '/').Replace('-', '+') + "=="))
        : Guid.Parse(value);
    }

    /// <summary>
    /// Creates a ShortGuid from a base64 encoded string OR any of the allowed Guid formats. Functionally equivalent to new ShortGuid(value).
    /// </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <exception cref="ArgumentNullException">	Thrown when value is null. </exception>
    /// <exception cref="ArgumentException">		Thrown when value is whitespace. </exception>
    /// <exception cref="FormatException">			Thrown when value is not a properly formatted Guid string. </exception>
    /// <param name="value">	The encoded Guid as a base64 string OR any of the allowed Guid formats. </param>
    /// <returns>	A new instance of a ShortGuid if the string could be parsed. </returns>
    public static ShortGuid Parse(string value)
    {
      return new ShortGuid(value);
    }

    /// <summary>
    /// Creates a ShortGuid from a base64 encoded string OR any of the allowed Guid formats. Will not throw exceptions, and will instead
    /// return ShortGuid.Empty if the string cannot be parsed.
    /// </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <param name="guid">			The encoded Guid as a base64 string OR any of the allowed Guid formats. </param>
    /// <param name="shortGuid">	[out] A new instance of a ShortGuid corresponding to the string if it could be parsed, otherwise
    /// 							ShortGuid.Empty. </param>
    /// <returns>	A new instance of a ShortGuid corresponding to the string if it could be parsed, otherwise ShortGuid.Empty. </returns>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "The intent of the Try pattern is to eat any underlying parsing errors")]
    public static bool TryParse(string guid, out ShortGuid shortGuid)
    {
      try
      {
        shortGuid = new ShortGuid(guid);
        return true;
      }
      catch
      {
        shortGuid = new ShortGuid();
        return false;
      }
    }

    /// <summary>	Converts this object to a standard unique identifier. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <returns>	This object as a Guid. </returns>
    public Guid ToGuid()
    {
      return this._guid;
    }


    /// <summary>	Guid casting operator that allows ShortGuids to be cast to Guids. </summary>
    /// <remarks>	6/26/2011. </remarks>
    /// <param name="shortGuid">	The ShortGuid instance. </param>
    public static implicit operator Guid(ShortGuid shortGuid)
    {
      return shortGuid._guid;
    }

    /// <summary>	String casting operator that allows ShortGuids to be cast to strings. </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <param name="shortGuid">	The ShortGuid instance. </param>
    public static implicit operator string(ShortGuid shortGuid)
    {
      return shortGuid.LazyResolvedValue();
    }

    /// <summary>	Equality operator for ShortGuids. </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <param name="instanceX">	The first instance to compare. </param>
    /// <param name="instanceY">	The second instance to compare. </param>
    /// <returns>	true if the parameters are considered equivalent. </returns>
    public static bool operator ==(ShortGuid instanceX, ShortGuid instanceY)
    {
      if ((object)instanceX == null) return (object)instanceY == null;
      return instanceX._guid == instanceY._guid;
    }

    /// <summary>	Inequality operator for ShortGuids. </summary>
    /// <remarks>	6/25/2011. </remarks>
    /// <param name="instanceX">	The first instance to compare. </param>
    /// <param name="instanceY">	The second instance to compare. </param>
    /// <returns>	true if the parameters are not considered equivalent. </returns>
    public static bool operator !=(ShortGuid instanceX, ShortGuid instanceY)
    {
      return !(instanceX == instanceY);
    }
  }
}