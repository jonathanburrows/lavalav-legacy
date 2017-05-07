namespace lvl.Ontology
{
    /// <summary>
    ///     Represents a model in a domain that can be persisted, and compared against other entities.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        ///     The primary key of the entity. Used in comparisons with other entities.
        /// </summary>
        public virtual int Id { get; set; }

        /// <summary>
        ///     Compares two entities based on reference and identity.
        /// </summary>
        /// <param name="obj">The object to be compared against the entity.</param>
        /// <returns>
        ///     If the other object is not an entity, false.
        ///     If the other object is null, then false.
        ///     If the other object has the same reference, then true.
        ///     If the other object is of a different type, then false.
        ///     If either object has an id of zero, then false.
        ///     If both objects have matching ids, then true.
        ///     Otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            var boxed = obj as Entity;

            if (ReferenceEquals(boxed, null))
            {
                return false;
            }
            else if (ReferenceEquals(this, boxed))
            {
                return true;
            }
            else if (GetType() != boxed.GetType())
            {
                return false;
            }
            else if (Id == 0 || boxed.Id == 0)
            {
                return false;
            }
            else
            {
                return Id == boxed.Id;
            }
        }

        /// <summary>
        ///     Will return the hash of the entity type concatonated with it's identifier.
        /// </summary>
        public override int GetHashCode()
        {
            var uniqueKey = GetType().ToString() + Id;
            return uniqueKey.GetHashCode();
        }

        /// <summary>
        ///     Compares two entities based on reference and identity.
        /// </summary>
        /// <param name="a">First entity to be compared.</param>
        /// <param name="b">Second entity to be compared.</param>
        /// <returns>
        ///     If both are null, then true.
        ///     If only 1 is null, then false.
        ///     If the two entities have the same reference, then true.
        ///     If the two entities are of different type, then false.
        ///     If either entity have an id of zero, then false.
        ///     If entities have matching ids, then true.
        ///     Otherwise, false.
        /// </returns>
        /// <remarks>Similar to Equals, but handles the case of the first arg being null.</remarks>
        public static bool operator ==(Entity a, Entity b)
        {
            if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            {
                return true;
            }
            else if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }
            else
            {
                return a.Equals(b);
            }
        }

        /// <summary>
        ///     The complement of ==
        /// </summary>
        /// <param name="a">First entity to be compared.</param>
        /// <param name="b">Second entity to be compared.</param>
        public static bool operator !=(Entity a, Entity b) => !(a == b);

        protected Entity() { }
    }
}
