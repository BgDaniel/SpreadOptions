import numpy as np
import matplotlib.pyplot as plt

t = 1.0
dt = .01
t = np.arange(0, t, dt)  
nb_t = len(t)

S = -0.1 * np.sin(4.0 * np.pi * t) + 2.0 - .5 *t


q_min = - 0.10
q_max = + 0.12


Q_min = .0
Q_max = .5
dQ = 0.005
Q = np.arange(Q_min, Q_max, dQ)
nb_Q = len(Q)

J = np.empty((nb_t, nb_Q), dtype=tuple)  

for i_Q in range(0, nb_Q):
    J[-1][i_Q] = (.0, None, None)

for i_t in range(nb_t-2, -1, -1):
    for i_Q, _Q in enumerate(Q):
        indices = []    

        k = 0
        while k * dQ <= q_max and _Q + k * dQ < Q_max:
            indices.append(k)
            k += 1

        k = -1
        while k * dQ >= q_min and _Q + k * dQ > Q_min:
            indices.append(k)
            k -= 1
            
        _J = []
        for _k in indices:
            J_next = J[i_t+1][i_Q + _k][0]
            _J.append((- _k * dQ * S[i_t] + J_next, _k * dQ, i_Q + _k))

        l = np.argmax([v[0] for v in _J])
        J[i_t][i_Q] = _J[l]

Q_op = np.zeros(nb_t)
q_op = np.zeros(nb_t)
v = np.zeros(nb_t)

i_Q = 30

for i_t in range(0, nb_t):
    v[i_t] = J[i_t][i_Q][0]
    Q_op[i_t] = Q[i_Q]
    q_op[i_t] = J[i_t][i_Q][1]
    i_Q = J[i_t][i_Q][2]

plt.plot(v, color='red', label='optimal control value')
plt.plot(Q_op, color='blue', label='total storage volume')
plt.plot(q_op, color='green', label='change in storage volume')
plt.plot(S, color='orange', label='price underlying')
plt.legend()
plt.show()




