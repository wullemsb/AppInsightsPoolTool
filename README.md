# App Insights IIS Application Pool Tool

This tool is designed to help you add the IIS Application Pool local user accounts to the following Windows groups:
	- Performance Monitor Users
	- Performance Log Users

This is necessary to allow the App Insights IIS Application Pool to collect performance counter data.

You can find the related blog post: https://bartwullems.blogspot.com/2022/05/azure-application-insights-collect.html


## Usage
Compile and run the tool on the web server where IIS is running. 