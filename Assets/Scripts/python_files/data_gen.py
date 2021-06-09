# Kourosh: This is just a code for development of calling external programs from C#

import torch
import time
import json
import sys

for i in torch.arange(50):
    s = json.dumps({"name" : "peyton_{}".format(i.item()), "index" : i.item() * 100})
    time.sleep(0.5)
    print(s)
    sys.stdout.flush()