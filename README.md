# MOT Lookup

## Table of Contents
1. [Overview](#overview)
2. [Installation](#installation)
3. [Usage](#usage)
4. [Resources](#resources)
5. [Contributors](#contributors)

## Overview

The **MOT Lookup** is a web app built with Blazor and .NET 6 that allows users to retrieve MOT vehicle details. By entering a vehicle's registration number, users can see details such as the:

•	Make
•	Model
•	Colour
•	MOT Expiry Date
•	Mileage at last MOT

This application uses the UK Government's MOT API for fetching the data.

## Installation

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) or newer

### Steps

1. Clone this repository:
   ```bash
   git clone https://github.com/Iblal/MOTLookup.git
   cd /MOTLookup/MOTLookup.Presentation
   ```

2. Run the application:
   ```bash
   dotnet run --project MOTLookup.Presentation
   ```

3. Open a browser and go to `https://localhost:7150/` to access the application.

## Usage

1. **Landing Page**: Enter a vehicle's registration number in the input field.
2. **Submit**: After entering a valid registration number, click submit to get vehicle details.
3. **View Results**: The application displays the make, model, colour, MOT expiry date, and mileage from the last MOT.

## Resources

- [UK Government MOT API Documentation](https://developer-portal.dvsa.gov.uk/)

## Contributors

- Iblal Ahmed (https://github.com/iblal)