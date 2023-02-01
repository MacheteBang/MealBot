# MealBot

Every day, my teenage daughter would come home from school and ask "what's for dinner, dad?", (usually before I heard "Hi!"), and as much as I enjoy having a conversation with her, I'd prefer to skip the dinner conversation! 😊

## 🤖 Enter MealBot

Mealbot was built to:
1. Get the "what's for dinner" information right into my daughter's hands
2. Understand [Azure Functions](https://learn.microsoft.com/en-us/azure/azure-functions/functions-overview)
3. Play with [Twilio](https://www.twilio.com/) to send out SMS messages

## ⚙ How It Workds

There are two simple REST API Azure Functions that allow one to enter the meals associated with a day / meal time and to get the meal for a specific day and meail time.  Additionally, there is a function that on a timer that pulls the dinner for that day and sends out SMS to each person on the list.
