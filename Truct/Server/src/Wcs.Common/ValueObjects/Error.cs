namespace Wcs.Common.ValueObjects;

public enum ErrorType
{
  Info = 0,
  Warning = 1,
  Critical = 2,
}

public enum ErrorStatus
{
  New = 0,
  InProcessing = 1,
  Resolved = 2,
}