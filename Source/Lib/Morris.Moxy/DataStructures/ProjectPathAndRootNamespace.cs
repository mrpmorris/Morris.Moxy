using Morris.Moxy.Helpers;

namespace Morris.Moxy.DataStructures;

internal readonly struct ProjectPathAndRootNamespace : IEquatable<ProjectPathAndRootNamespace>
{
	public readonly string ProjectPath;
	public readonly string RootNamespace;

	private readonly Lazy<int> CachedHashCode;

	public ProjectPathAndRootNamespace(string projectPath, string rootNamespace)
	{
		ProjectPath = projectPath ?? throw new ArgumentNullException(nameof(projectPath));
		RootNamespace = rootNamespace ?? throw new ArgumentNullException(nameof(rootNamespace));
		CachedHashCode = new Lazy<int>(() => HashCode.Combine(projectPath, rootNamespace));
	}

	public override int GetHashCode() => CachedHashCode.Value;

	public override bool Equals(object obj) =>
		obj is ProjectPathAndRootNamespace other && Equals(other);

	public bool Equals(ProjectPathAndRootNamespace other) =>
		ProjectPath == other.ProjectPath
		&& RootNamespace == other.RootNamespace;

	public static bool operator ==(ProjectPathAndRootNamespace left, ProjectPathAndRootNamespace right) => left.Equals(right);

	public static bool operator !=(ProjectPathAndRootNamespace left, ProjectPathAndRootNamespace right) => !(left == right);
}

