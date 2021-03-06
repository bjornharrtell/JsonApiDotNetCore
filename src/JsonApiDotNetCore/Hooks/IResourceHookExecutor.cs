using System.Collections.Generic;
using JsonApiDotNetCore.Models;
using JsonApiDotNetCore.Services;

namespace JsonApiDotNetCore.Hooks
{
    /// <summary>
    /// Transient service responsible for executing Resource Hooks as defined 
    /// in <see cref="ResourceDefinition{T}"/>. see methods in 
    /// <see cref="IReadHookExecutor"/>, <see cref="IUpdateHookExecutor"/> and 
    /// <see cref="IOnReturnHookExecutor"/> for more information.
    /// 
    /// Uses <see cref="TraversalHelper"/> for traversal of nested resource data structures.
    /// Uses <see cref="HookExecutorHelper"/> for retrieving meta data about hooks, 
    /// fetching database values and performing other recurring internal operations.
    /// </summary>
    public interface IResourceHookExecutor : IReadHookExecutor, IUpdateHookExecutor, ICreateHookExecutor, IDeleteHookExecutor, IOnReturnHookExecutor { }

    public interface ICreateHookExecutor
    {
        /// <summary>
        /// Executes the Before Cycle by firing the appropriate hooks if they are implemented. 
        /// The returned set will be used in the actual operation in <see cref="JsonApiResourceService{TResource}"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.BeforeCreate"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.BeforeUpdateRelationship"/>
        /// hook for any secondary (nested) resource for values within parameter <paramref name="resources"/>
        /// </summary>
        /// <returns>The transformed set</returns>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        IEnumerable<TResource> BeforeCreate<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
        /// <summary>
        /// Executes the After Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.AfterCreate"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.AfterUpdateRelationship"/>
        /// hook for any secondary (nested) resource for values within parameter <paramref name="resources"/>
        /// </summary>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        void AfterCreate<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
    }

    public interface IDeleteHookExecutor
    {
        /// <summary>
        /// Executes the Before Cycle by firing the appropriate hooks if they are implemented. 
        /// The returned set will be used in the actual operation in <see cref="JsonApiResourceService{TResource}"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.BeforeDelete"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.BeforeImplicitUpdateRelationship"/>
        /// hook for any resources that are indirectly (implicitly) affected by this operation.
        /// Eg: when deleting a resource that has relationships set to other resources, 
        /// these other resources are implicitly affected by the delete operation.
        /// </summary>
        /// <returns>The transformed set</returns>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        IEnumerable<TResource> BeforeDelete<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;

        /// <summary>
        /// Executes the After Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.AfterDelete"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// </summary>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <param name="succeeded">If set to <c>true</c> the deletion succeeded.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        void AfterDelete<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline, bool succeeded) where TResource : class, IIdentifiable;
    }

    /// <summary>
    /// Wrapper interface for all Before execution methods.
    /// </summary>
    public interface IReadHookExecutor
    {
        /// <summary>
        /// Executes the Before Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.BeforeRead"/>
        /// hook where T = <typeparamref name="TResource"/> for the requested 
        /// resources as well as any related relationship.
        /// </summary>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <param name="stringId">StringId of the requested resource in the case of
        /// <see cref="JsonApiResourceService{TResource,TId}.GetAsync(TId)"/>.</param>
        /// <typeparam name="TResource">The type of the request resource</typeparam>
        void BeforeRead<TResource>(ResourcePipeline pipeline, string stringId = null) where TResource : class, IIdentifiable;
        /// <summary>
        /// Executes the After Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.AfterRead"/> for every unique
        /// resource type occuring in parameter <paramref name="resources"/>.
        /// </summary>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        void AfterRead<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
    }

    /// <summary>
    /// Wrapper interface for all After execution methods.
    /// </summary>
    public interface IUpdateHookExecutor
    {
        /// <summary>
        /// Executes the Before Cycle by firing the appropriate hooks if they are implemented. 
        /// The returned set will be used in the actual operation in <see cref="JsonApiResourceService{TResource}"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{TResource}.BeforeUpdate(IDiffableResourceHashSet{TResource}, ResourcePipeline)"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.BeforeUpdateRelationship"/>
        /// hook for any secondary (nested) resource for values within parameter <paramref name="resources"/>
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.BeforeImplicitUpdateRelationship"/>
        /// hook for any resources that are indirectly (implicitly) affected by this operation.
        /// Eg: when updating a one-to-one relationship of a resource which already 
        /// had this relationship populated, then this update will indirectly affect 
        /// the existing relationship value.
        /// </summary>
        /// <returns>The transformed set</returns>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        IEnumerable<TResource> BeforeUpdate<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
        /// <summary>
        /// Executes the After Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.AfterUpdate"/>
        /// hook where T = <typeparamref name="TResource"/> for values in parameter <paramref name="resources"/>.
        /// <para />
        /// Fires the <see cref="ResourceDefinition{U}.AfterUpdateRelationship"/>
        /// hook for any secondary (nested) resource for values within parameter <paramref name="resources"/>
        /// </summary>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        void AfterUpdate<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
    }

    /// <summary>
    /// Wrapper interface for all On execution methods.
    /// </summary>
    public interface IOnReturnHookExecutor
    {
        /// <summary>
        /// Executes the On Cycle by firing the appropriate hooks if they are implemented. 
        /// <para />
        /// Fires the <see cref="ResourceDefinition{T}.OnReturn"/> for every unique
        /// resource type occuring in parameter <paramref name="resources"/>.
        /// </summary>
        /// <returns>The transformed set</returns>
        /// <param name="resources">Target resources for the Before cycle.</param>
        /// <param name="pipeline">An enum indicating from where the hook was triggered.</param>
        /// <typeparam name="TResource">The type of the root resources</typeparam>
        IEnumerable<TResource> OnReturn<TResource>(IEnumerable<TResource> resources, ResourcePipeline pipeline) where TResource : class, IIdentifiable;
    }
}
