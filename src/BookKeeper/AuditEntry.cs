using System;
using System.Collections.Generic;

namespace BookKeeper
{
  public class AuditEntry
  {
    /// <summary>
    /// List of strings representing path to delete.
    /// </summary>
    public List<string> Deletes { get; set; }
    /// <summary>
    /// List of K/V pairs, where Key is the path to the property and Value is the updated property value.
    /// </summary>
    public List<KeyValuePair<string, string>> Updates { get; set; }
    /// <summary>
    /// DateTime when the AuditEntry was created.
    /// </summary>
    public DateTime TimeStamp { get; set; }
    /// <summary>
    /// UserId of the user that made changes.
    /// </summary>
    public string UserId { get; set; }
  }
}
