#ifndef AP_DOWRITE_CLAMP_INCLUDED
#define AP_DOWRITE_CLAMP_INCLUDED

bool in01(float2 pos)
{
    return (pos.x >= 0 && pos.x <= 1) && (pos.y >= 0 && pos.y <= 1);
}

#endif