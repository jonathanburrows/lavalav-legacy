using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;
using System.Linq;

namespace lvl.Ontology.Conventions
{
    /// <summary>
    ///     Provides error messages at startup when references do not adhere to domain driven design.
    /// </summary>
    /// <remarks>
    ///     This was done to prevent long term degredation of the domain model. While not critical, does help with forward planning.
    /// </remarks>
    internal class InvalidAggregateRootReferenceConvention : IReferenceConvention, IReferenceConventionAcceptance, IHasManyConvention, IHasManyConventionAcceptance
    {
        public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
        {
            criteria.Expect(inspector =>
            {
                var referencedType = inspector.Class.GetUnderlyingSystemType();
                return !ReferencedEntityIsInAggregateScope(inspector.EntityType, referencedType);
            });
        }

        public void Apply(IManyToOneInstance instance)
        {
            throw new InvalidOperationException($"The class {instance.EntityType.Name} references {instance.Class.Name}, which does not implement {nameof(IAggregateRoot)}, or is not part of an AggregateScope<{instance.EntityType.Name}>");
        }

        public void Accept(IAcceptanceCriteria<IOneToManyCollectionInspector> criteria)
        {
            criteria.Expect(inspector => !ReferencedEntityIsInAggregateScope(inspector.EntityType, inspector.ChildType));
        }

        public void Apply(IOneToManyCollectionInstance instance)
        {
            throw new InvalidOperationException($"The class {instance.EntityType.Name} references {instance.ChildType.Name}, which does not implement {nameof(IAggregateRoot)}, or is not part of an AggregateScope<{instance.EntityType.Name}>");
        }

        /// <summary>
        ///     Will mark references between two entities invalid if they are not within the same scope.
        /// </summary>
        /// <param name="parentType">The type which contains the reference.</param>
        /// <param name="referencedType">The type which is being referenced.</param>
        /// <returns>
        ///     If the referenced entity is an AggregateRoot, then true.
        ///     
        ///     If the referenced entity doesnt have an AggregateScope, then false.
        ///     
        ///     If the parent entity is an aggregate root and the referenced entity belongs to its scope, then true.
        ///     
        ///     If the parent entity and referenced entity belong to the same scope, then true.
        ///     
        ///     Otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="parentType"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="referencedType"/> is null.</exception>
        private bool ReferencedEntityIsInAggregateScope(Type parentType, Type referencedType)
        {
            if (parentType == null)
            {
                throw new ArgumentNullException(nameof(parentType));
            }
            if (referencedType == null)
            {
                throw new ArgumentNullException(nameof(referencedType));
            }

            if (typeof(IAggregateRoot).IsAssignableFrom(referencedType))
            {
                // referenced type is an aggregate root, and references to aggregate roots are always allowed.
                return true;
            }

            var referencedInterfaces = referencedType.GetInterfaces().Where(i => i.IsGenericType);
            var referencedAggregateScope = referencedInterfaces.SingleOrDefault(i => i.GetGenericTypeDefinition() == typeof(IAggregateScope<>));
            if (referencedAggregateScope == null)
            {
                // referenced type doesnt belong to an aggregate scope, so it cant be valid.
                return false;
            }
            else if (typeof(IAggregateRoot).IsAssignableFrom(parentType))
            {
                // the parent class is an aggregate root, and references are allowed to entities within its aggregate scope.
                var aggregateRoot = referencedAggregateScope.GetGenericArguments().Single();
                return aggregateRoot == parentType;
            }
            else
            {
                // references will only be allowed if the referenced type shares the aggregate scope
                var entityInterfaces = parentType.GetInterfaces().Where(i => i.IsGenericType);
                var entityAggregateScope = entityInterfaces.Single(i => i.GetGenericTypeDefinition() == typeof(IAggregateScope<>));
                return entityAggregateScope == referencedAggregateScope;
            }
        }
    }
}
