# Problems that I see with our current design: 
1. Asset manager only loads production units that are specified in the list on Asset manager initialization. 
It is bad, because then it will be impossible to choose diffrent units in the app's interface. 

2. We need a method that goes through the whole HourlyData list and calls DistributeHeatLoad(), returning final resultData. 

3. Gotta move all the things related to SQL to a separate branch and remove them from main. 

4. Do we ever need to save a single resultData actually, or should it becomne private and only SaveMany be exposed as public?  

5. Remember YAGNI principles when designing the app. We seem to create a variety of methods, but not all of them might actually necessary.

6. Optimizer should not depend on a ResultManager or even IResultManager I think, because then it is hard to make it reusable for anything else. 