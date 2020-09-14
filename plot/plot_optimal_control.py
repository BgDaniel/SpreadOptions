import pandas as pd
import numpy as np
import os
from pathlib import Path
import matplotlib.pyplot as plt

'''
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
'''

# get data as pandas dataframe
DIR_PATH = Path(__file__).parent.parent
PATH_TO_DATA = os.path.join(DIR_PATH, "StatDynHedging\\Data\\tracking_errors_both_dynamic.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_tracking_errors_both_dynamic = samples.TrackingErr

PATH_TO_DATA = os.path.join(DIR_PATH, "StatDynHedging\\Data\\tracking_errors_both_static.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_tracking_errors_both_static = samples.TrackingErr

PATH_TO_DATA = os.path.join(DIR_PATH, "StatDynHedging\\Data\\tracking_errors_G_dynamic.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_tracking_errors_G_dynamic = samples.TrackingErr

PATH_TO_DATA = os.path.join(DIR_PATH, "StatDynHedging\\Data\\tracking_errors_P_dynamic.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')
tracking_error_tracking_errors_P_dynamic = samples.TrackingErr

# plot data
plt.plot(tracking_error_tracking_errors_both_dynamic, color='red', label='both dynamic')
plt.plot(tracking_error_tracking_errors_both_static, color='blue', label='both static')
plt.plot(tracking_error_tracking_errors_G_dynamic, color='yellow', label='gas dynamic')
plt.plot(tracking_error_tracking_errors_P_dynamic, color='green', label='power dynamic')
plt.legend()
plt.title = 'Tracking Error Delta vs Gamma Hedge'
plt.show()
