using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace WpfExtensions.Binding;

/// <summary>
/// Provides API for reactive feature, which is an implementation of <see cref="IReactivity"/>.
/// </summary>
public class Reactivity : IReactivity
{
    /// <summary>
    /// Gets a default instance of <see cref="IReactivity"/>.
    /// </summary>
    public static IReactivity Default { get; } = new Reactivity();

    /// <inheritdoc/>
    public IDisposable Watch<T>(Expression<Func<T>> expression, Action callback)
    {
        DependencyGraph graph = GenerateDependencyGraph(expression);
        IDisposable[] tokens = graph.DependencyRootNodes
            .Select(item => item.Initialize(OnPropertyChanged))
            .Concat(graph.ConditionalRootNodes.Select(item => item.Initialize()))
            .ToArray();

        IDisposable token = Disposable.Create(() =>
        {
            foreach (IDisposable token in tokens)
            {
                token.Dispose();
            }
        });

        Binding.Scope.ActiveEffectScope?.AddStopToken(token);
        return token;

        void OnPropertyChanged(object sender, EventArgs e) => callback();
    }

    /// <inheritdoc/>
    public Scope Scope(bool detached = false) => new(detached);

    private static DependencyGraph GenerateDependencyGraph<T>(Expression<Func<T>> expression)
    {
        var visitor = new SingleLineLambdaVisitor();
        visitor.Visit(expression);
        return new DependencyGraph(visitor.RootNodes, visitor.ConditionalNodes);
    }
}
