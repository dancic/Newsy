using Newsy.Abstractions.Enums;

namespace Newsy.Abstractions.Models;
public record Filter(string Field, OperatorType Type, string Value = "");
