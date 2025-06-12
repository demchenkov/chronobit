namespace Razor.Models;

[Flags]
public enum AdministrationAction : UInt64
{
  All = UInt64.MaxValue,
  None = UInt64.MinValue,

  Update = 1 << 0,
  Delete = 1 << 1,
  ManageWorkers = 1 << 2,

  // Max possible value is 1 << 63
}
