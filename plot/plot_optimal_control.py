import pandas as pd
import numpy as np
import os
from pathlib import Path
import matplotlib.pyplot as plt

# get data as pandas datafram
DIR_PATH = Path(__file__).parent.parent
PATH_TO_DATA = os.path.join(DIR_PATH, "HedgingStrategies\\Data\\hedge_along_path.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')

# plot data
plt.plot(samples.ValueHedge, color='red', label='value hedge')
plt.plot(samples.ValueAnalytical, color='blue', label='analytical value')
plt.legend()
plt.show()
