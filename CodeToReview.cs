/*
PR Review - Actionable Summary

File purpose: generate and store person records, filter by name, and compose married names.

Top-priority issues (block merging):
- Typo in using directive: `System.Collegctions.Generic` (fix to `System.Collections.Generic`).
- Model naming: `People` represents a single person — rename to `Person`.
- Randomness: `new Random()` inside the loop and `random.Next(0, 1)` cause biased/repeated results; reuse/inject `Random` and use `Next(2)`.
- Date/time mismatch: mixing `DateTime` and `DateTimeOffset` and using `.Date` discards offsets; use `DateTimeOffset` consistently.
- `GetMarried` is broken: invalid length check, discarded substring, and undocumented special-case behavior.
- Exceptions are swallowed; the original exception and stack trace are lost.
- Internal state is exposed by returning the mutable `_people` list.

Suggested minimal fixes (apply before approving):
1. Fix the using directive typo.
2. Rename `People` → `Person` and make DOB a `DateTimeOffset` throughout.
3. Reuse or inject a single `Random` instance and change `random.Next(0,1)` → `random.Next(0,2)`.
4. Compute DOB using `DateTimeOffset.UtcNow.AddYears(-age)` instead of `TimeSpan` multiplication.
5. Fix `GetMarried` to validate inputs and return a correctly truncated `fullName`.
6. Preserve exception details when rethrowing or remove unnecessary try/catch.
7. Return `_people.AsReadOnly()` or a copy instead of exposing the mutable list.

Acceptance criteria:
- Code compiles.
- Random selection and DOB generation behave as expected.
- `GetMarried` returns truncated values when necessary and documents any special rules.
- Exceptions include original diagnostic details and internal state is not exposed.

*/

using System;
using System.Collegctions.Generic; 
using System.Linq;
//If above imports are not used, they should be removed.

namespace Utility.Valocity.ProfileHelper
{
    public class People //People is a single entity, so Person is a better name. Follow singular naming convention for entities/models.
    {
     private static readonly DateTimeOffset Under16 = DateTimeOffset.UtcNow.AddYears(-15);
     public string Name { get; private set; }
     public DateTimeOffset DOB { get; private set; }
     public People(string name) : this(name, Under16.Date) { }
     public People(string name, DateTime dob) {
         Name = name;
         DOB = dob;
     }
     //DateTime dob vs DateTimeOffset DOB is inconsistent.
     }

    public class BirthingUnit //BirthingUnit is a confusing class name for a random person generator. It does not convey the actual responsibility.
    {
        /// <summary>
        /// MaxItemsToRetrieve
        /// </summary>
        private List<People> _people;

        public BirthingUnit()
        {
            _people = new List<People>();
        }

        /// <summary>
        /// GetPeoples
        /// </summary>
        /// <param name="j"></param>
        /// <returns>List<object></returns>
        
        public List<People> GetPeople(int i) //GetPeople(int i) should use a meaningful parameter name like count.
        {
            for (int j = 0; j < i; j++)
            {
                try
                {
                    // Creates a dandon Name
                    string name = string.Empty;
                    var random = new Random();
                    if (random.Next(0, 1) == 0) {
                        name = "Bob";
                    }
                    else {
                        name = "Betty";
                    }
                    // Adds new people to the list
                    _people.Add(new People(name, DateTime.UtcNow.Subtract(new TimeSpan(random.Next(18, 85) * 356, 0, 0, 0))));
                }
                catch (Exception e)
                {
                    // Dont think this should ever happen
                    throw new Exception("Something failed in user creation");
                }
            }
            return _people;
        }

        private IEnumerable<People> GetBobs(bool olderThan30) //GetBobs should be public if it is intended to be used outside of this class. Also, the method name should be more descriptive, like GetBobsOlderThan30.
        {
            return olderThan30 ? _people.Where(x => x.Name == "Bob" && x.DOB >= DateTime.Now.Subtract(new TimeSpan(30 * 356, 0, 0, 0))) : _people.Where(x => x.Name == "Bob");
        }

        public string GetMarried(People p, string lastName)
        {
            if (lastName.Contains("test"))
                return p.Name;
            if ((p.Name.Length + lastName).Length > 255)
            {
                (p.Name + " " + lastName).Substring(0, 255);
            }

            return p.Name + " " + lastName;
        }
    }
}