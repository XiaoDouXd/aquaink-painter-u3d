            float4 update_adv(AP_D2Q9_Fi fi, float d_rho, sampler2D last, float2 uv)
            {
                float4 la = tex2D(last, uv);
                float k = fi.f0 * la.w * d_rho;
                const float k0 = 1-k;
                float o = k;
                float3 col0 = la.xyz;
            
                // 拿01234方向的浓度更新我的颜色和浓度
                la = tex2D(last, uv + float2(-_Delta.x, 0));
                k = fi.f1234.x * la.w * d_rho;
                float3 coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(0, -_Delta.y));
                k = fi.f1234.y * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(_Delta.x, 0));
                k = fi.f1234.z * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(0, _Delta.y));
                k += fi.f1234.w * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
            
                // 拿5678方向来更新我的浓度
                la = tex2D(last, uv + float2(-_Delta.x, -_Delta.y));
                k = fi.f5678.x * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(_Delta.x, -_Delta.y));
                k = fi.f5678.y * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(_Delta.x, _Delta.y));
                k = fi.f5678.z * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                // ----------
                la = tex2D(last, uv + float2(-_Delta.x, _Delta.y));
                k = fi.f5678.w * la.w * d_rho;
                coln = la.xyz;
                col0 = ap_mixbox_kmerp(col0, coln, k, _ColTable, sampler_ColTable);
                o += k;
                
                return float4(col0, o);
            }

            // float4 col = tex2D(_Last, i.uv);
                // // 考察周边的速度
                // // 1234 方向
                // get_ChangeCol(i.uv + float2(_Delta.x, 0), col);
                // get_ChangeCol(i.uv + float2(0, _Delta.y), col);
                // get_ChangeCol(i.uv + float2(-_Delta.x, 0), col);
                // get_ChangeCol(i.uv + float2(0, -_Delta.y), col);
                //
                // // 5678 方向
                // get_ChangeCol(i.uv + float2(_Delta.x, _Delta.y), col);
                // get_ChangeCol(i.uv + float2(-_Delta.x, _Delta.y), col);
                // get_ChangeCol(i.uv + float2(-_Delta.x, -_Delta.y), col);
                // get_ChangeCol(i.uv + float2(_Delta.x, -_Delta.y), col);
                //
                // return col;