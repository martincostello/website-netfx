// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSuppressions.cs" company="http://www.martincostello.com">
//   Martin Costello (c) 2014
// </copyright>
// <summary>
//   GlobalSuppressions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MartinCostello", Justification = "Follows the MVC pattern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MartinCostello.Controllers", Justification = "Follows the MVC pattern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MartinCostello.Filters", Justification = "Follows the MVC pattern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "MartinCostello.Models.Identity", Justification = "Keeps the identity-related types separate.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.Models.GuidFormat.set_Text(System.String)", Scope = "member", Target = "MartinCostello.Models.GenerateGuidModel.#.cctor()", Justification = "Not a concern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.Models.HashType.set_Text(System.String)", Scope = "member", Target = "MartinCostello.Models.GenerateHashModel.#.cctor()", Justification = "Not a concern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "MartinCostello.Models.HashType.set_Text(System.String)", Scope = "member", Target = "MartinCostello.Models.GenerateMachineKeyModel.#.cctor()", Justification = "Not a concern.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames", Justification = "Owin.Security.Providers cannot be used without it as it is not strong-named.")]
