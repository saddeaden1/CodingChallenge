# CodingChallenge

How to build the code

The code can be build in visual studio.

A local.settings.json file is needed to be added to the directory CodingChallenge\CodingChallenge\bin\Debug\net6.0 to run with the json 

{
    "Key":"your_developer_key"
}

How to run the output

The exe is run by how it is done in the document I was given, hitting the exe in the cmd with a road, only one can be entered if multiple ar entered it is rejected.

The exe is in CodingChallenge\bin\Debug\net6.0

The assumtions I have made are as follows:

I assumed app key is obsolete as I couldn't find it on the website and this forum made me belive it is not used anymore. I could have included it in the header handler as a second header to add which is a quick fix but since i belive it is obselete i didn't add it.
https://techforum.tfl.gov.uk/t/launch-of-new-developer-portal-faq/1556

The API returns only the 2 types of object given in the document given to me. 
Assumed the roads api has all the valid roads that can be entered so used it to validate the road name.
I assumed the road names will not change so used the road A2 in my inegration tests to test a valid road name entry.

How to run any tests that you have written:

local.settings.json file is needed to be added to the directory CodingChallenge\CodingChallenge.IntegrationTests\bin\Debug\net6.0 to run the integration tests with the json

{
    "Key":"your_developer_key"
}

The tests can be run in the visaul studio test runner.