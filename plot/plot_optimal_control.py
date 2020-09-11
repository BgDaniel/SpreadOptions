import pandas as pd
import numpy as np
import os
from pathlib import Path
import matplotlib.pyplot as plt


# get data as pandas dataframe
DIR_PATH = Path(__file__).parent.parent
PATH_TO_DATA = os.path.join(DIR_PATH, "StatDynHedging\\Data\\delta_hedge_along_path.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')

# plot data
plt.plot(samples.ValueHedge, color='red', label='value hedge')
plt.plot(samples.ValueAnalytical, color='blue', label='analytical value')
plt.legend()
plt.title = 'Delta Hedge Along Path'
plt.show()

'''
# get data as pandas dataframe
DIR_PATH = Path(__file__).parent.parent
PATH_TO_DATA = os.path.join(DIR_PATH, "HedgingStrategies\\Data\\delta_hedge_along_path.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')

# plot data
plt.plot(samples.ValueHedge, color='red', label='value hedge')
plt.plot(samples.ValueAnalytical, color='blue', label='analytical value')
plt.legend()
plt.title = 'Delta Hedge Along Path'
plt.show()

# get data as pandas dataframe
PATH_TO_DATA = os.path.join(DIR_PATH, "HedgingStrategies\\Data\\gamma_hedge_along_path.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')

# plot data
plt.plot(samples.ValueHedge, color='red', label='value hedge')
plt.plot(samples.ValueAnalytical, color='blue', label='analytical value')
plt.legend()
plt.title = 'Gamma Hedge Along Path'
plt.show()

# get data as pandas dataframe
PATH_TO_DATA = os.path.join(DIR_PATH, "HedgingStrategies\\Data\\delta_hedge_tracking_errors.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_delta_hedge = samples.TrackingErr

PATH_TO_DATA = os.path.join(DIR_PATH, "HedgingStrategies\\Data\\gamma_hedge_tracking_errors.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_gamma_hedge = samples.TrackingErr

# plot data
plt.plot(tracking_error_delta_hedge, color='red', label='delta hedge')
plt.plot(tracking_error_gamma_hedge, color='blue', label='gamma hedge')
plt.legend()
plt.title = 'Tracking Error Delta vs Gamma Hedge'
plt.show()
'''