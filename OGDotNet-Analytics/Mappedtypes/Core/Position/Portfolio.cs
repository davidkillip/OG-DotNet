﻿namespace OGDotNet_Analytics.Mappedtypes.Core.Position
{
    public interface IPortfolio
    {
        string Identifier { get; }
        string Name { get;  }
        PortfolioNode Root { get;  }
    }
}