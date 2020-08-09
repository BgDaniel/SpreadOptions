import pandas as pd
import numpy as np
import os
from pathlib import Path
import matplotlib.pyplot as plt

# get data as pandas datafram
DIR_PATH = Path(__file__).parent.parent
PATH_TO_DATA = os.path.join(DIR_PATH, "StochasticControl\\Data\\q_Steps.csv")
samples = pd.read_csv(PATH_TO_DATA, sep=';')

# plot data
plt.plot(samples.Value, color='red', label='optimal control value')
plt.plot(samples.Q, color='blue', label='total storage volume')
plt.plot(samples.Dq, color='green', label='change in storage volume')
plt.plot(samples.S, color='orange', label='price underlying')
plt.legend()
plt.show()
