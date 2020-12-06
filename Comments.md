1. I have created a set of unit tests to cover the functionality before rework
	the existing unit test consisted of system under test which was a Mock, I have changed it to have sut what it supposed to be.
2. Added DTOHelper to encapsulate reacurring code for creating Company data
3. Added Constants to hold const values like error messages or response codes (this will allow to have consistent responses)
4. Encapsulated interpretation of the response into DTOHelper. in case of different types of results we can extend it further there

I thought about remaining logic in the function but for now it looks quite readible and managable.

