using EqualityComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookKeeper
{
  public class Audited<T>
  {
    public Audited(T obj)
    {
      this.Original = obj;
      this.AuditLog = new List<AuditEntry>();
    }

    private static GenericEqualityComparer<AuditEntry> idOnlyComparer = new GenericEqualityComparer<AuditEntry>((a1, a2) => a1.Id == a2.Id);
    public T Original { get; private set; }

    /// <summary>
    /// Instance Merge, generally used for simple updates. Gets diff of object and adds to AuditLog.
    /// </summary>
    /// <param name="obj">The new version of the object</param>
    /// <param name="meta">To keep metadata with the AuditEntry, such as user information.</param>
    /// <returns>A representation on the object as Audited</returns>
    public Audited<T> Merge(T obj, Dictionary<string,string> meta)
    {
      //Set up new Audited<T> for merge
      Audited<T> mergedAuditedObj = new Audited<T>(this.Original);
      mergedAuditedObj.AuditLog = this.AuditLog;

      //Get new AuditEntry
      AuditEntry auditEntry = GetAuditEntry(this, obj);
      auditEntry.Meta = meta;
      
      //Add AuditEntry to Audited<T> and return
      mergedAuditedObj.AuditLog.Add(auditEntry);
      return mergedAuditedObj;
    }

    /// <summary>
    /// Instance Merge, generally used for simple updates. Gets diff of object and adds to AuditLog.
    /// </summary>
    /// <param name="obj">The new version of the object</param>
    /// <returns>A representation on the object as Audited</returns>
    public Audited<T> Merge(T obj)
    {
      return (Merge(obj, null));
    }

    /// <summary>
    /// Static Merge, generally used to merge siblings. Sums and orders all AuditLogs.
    /// </summary>
    /// <param name="objects">The objects to merge</param>
    /// <returns>Merged Audited<T></returns>
    public static Audited<T> Merge(List<Audited<T>> objects)
    {
      if (objects.Count < 2) { throw new ArgumentException("Must merge 2 or more objects", "objects"); }
      var merged = new Audited<T>(objects[0].Original);
      merged.AuditLog = objects.SelectMany(o => o.AuditLog).Distinct(idOnlyComparer).OrderBy(o => o.TimeStamp).ToList();
      return merged;
    }

    /// <summary>
    /// List of AuditEntry reflecting all changes over time.
    /// </summary>
    public List<AuditEntry> AuditLog { get; set; }

    /// <summary>
    /// Static implicit conversion operator, allowing Audited<T> to be used as T without cast/convert.
    /// </summary>
    /// <param name="audited"></param>
    /// <returns>Inner T object</returns>
    public static implicit operator T(Audited<T> audited)
    {
      var returnObj = new Audited<T>(audited.Original);

      audited.AuditLog.OrderBy(e => e.TimeStamp).ToList().ForEach(e =>
      {
        e.Deletes.ForEach(d =>
        {
          //TODO: Once we know how paths and diffs will work, implement replaying deletes
        });
        e.Updates.ForEach(u =>
        {
          //TODO: Once we know how paths and diffs will work, implement replaying adds/updates
        });
      });
      return returnObj.Original;
    }

    /// <summary>
    /// Calculates differences between two objects and creates AuditEntry based on them.
    /// </summary>
    /// <param name="then">The original object</param>
    /// <param name="now">The updated object</param>
    /// <returns>AuditEntry with changes between then and now.</returns>
    private AuditEntry GetAuditEntry(T then, T now)
    {
      //TODO: Get some kind of sync'd time thing working
      var auditEntry = new AuditEntry();
      //TODO: Once EqualityComparer implements diffing stuff, call that
      // e.g. 
      //      var diffs = EqualityComparer.GetDifferences(then, now);
      //      diffs.ForEach(d =>
      //        {
      //          if (d.DiffType == DiffTypes.Delete) {auditEntry.Deletes.Add(d.Path);}
      //          else {auditEntry.Updates.Add(d.Path, d.Json);}
      //        });
      return auditEntry;
    }
  }
}
