# Bakery App
Purpose: To create an app designed to gather user-inputted data and store in a database. Additionally, it will provide a report based off
user specifications.

This application was designed to help an employee keep track of all the invoices and data at the barkery.

Usage:

User inputs specific quantities of each cake and will generate the total cost of that specific invoice, either by pushing a button to calculate before saving or will auto-calculate upon saving. The data is stored into the 'Customers' table in Microsoft Access DB. Additional customers can be added to the database as well. Next, user can also adjust the prices of each cake and will load the same prices upon application launch. This is also handled by storing the prices in the 'CakePrices' table in MS DB. The invoice number and the date of the invoice is auto generated. It is to prevent the user having to remember what invoice number was next. If user has made a mistake in the invoice and accidently saves it, user can retrieve that data and update it. Lastly, the application can generate a report into Microsoft Excel based off user spcifications, e.g. date range.

Notes: 
-Provides the MS DB file within the project.

-The application currently reads the Access file in .mdb format. Other extentions currently do not work.
