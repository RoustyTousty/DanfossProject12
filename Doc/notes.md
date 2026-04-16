# Problems that I see with our current design: 

- Asset manager only loads production units that are specified in the list on Asset manager initialization. 
It is bad, because then it will be impossible to choose diffrent units in the app's interface. Instead, it should load all production units and have a parameter of `bool active`, and have a method GetActiveProductionUnits() that return a list of ProductionUnits that are active


- Since there is a specific order of actions, maybe it would be useful to create a facade object which does those things together. 


- In RDM, we have an async method SaveMany(). How is it going to work when Save() is an async function and it also handles the parsing? I think that parsing should be a separate non-async function, and then Save() and SaveMany() should save strings provided by that method and write it down. Actually, why do we need Save() if we can do SaveMany() on a single piece of resultData, achieving the same results?

- In CsvResultRepository, there is no check for a valid fileName / path name

- UnitProduction is not currently being saved properly nor it is loaded from resultData

