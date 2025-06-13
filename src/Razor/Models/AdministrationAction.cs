namespace Razor.Models;

[Flags]
public enum AdministrationAction : UInt64
{
  All = UInt64.MaxValue,
  None = UInt64.MinValue,

  ViewShifts = 1 << 0,
  ManageShifts = 1 << 1,

  ManageWorkers = 1 << 2,

  Update = 1 << 3,
  Delete = 1 << 4,

  // Max possible value is 1 << 63
}
