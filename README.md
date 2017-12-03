# Core of a community debts register
Intended for:
* registration of debt deals between many people;
* quick retrieval of up-to-date due debts and statistics.

Each person can both lend and borrow. 

Currently usable by single data entry operator, but has partial support for concurrent use.

## Data provided by API
* people:
   * person: id, name, surname
* debts:
   * debt deals
      * deal: id, time, giverId, takerId, amount
      * credits
      * paybacks
   * person:
      * totals:
         * total amount historically credited by person
         * total amount historically owed by person
         * total count of credits taken
      * statistics:
         * person's average historical debt through cases of borrowing
      * fraction of repaid debts
   * pair:
      * current debt
   * achievers:
      * who historically credited max total
      * with max due debts total
      * best debtor by repaid fraction then total

## Further development
* implement asynchronous API methods
* finish with concurrency control - either using shared serializer or optimistic concurrency control
* improve DI, make more tests etc. (see [info/TODOs.txt](https://github.com/karlis-repsons/debts-register/blob/master/info/TODOs.txt))

## Implementation notes
### Techniques
Dependency injection, transactions, code-first tables database, in-memory databases for testing.
### Technologies
MS Entity Framework Core, MongoDB, xUnit.
### Structure
For easier, but still detailed overview of code structure, see mindmap file [info/DebtsRegister structure.xmind](https://github.com/karlis-repsons/debts-register/blob/master/info/DebtsRegister%20structure.xmind).
