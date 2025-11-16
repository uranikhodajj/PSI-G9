# Information Security

### Introductory Information  
**University of Prishtina — Faculty of Computer and Software Engineering**  
**Master’s Program in Computer and Software Engineering**  
**Professor:** Dr.Sc. Mërgim Hoti  
**Group:** 9  

---

### Team Members  
- Rina Bunjaku
- Uranik Hodaj 

---

### Project Overview  
Testimi i algoritmit për HASH-ing SHA-256 në C#

### Results
=== NIST test vectors për SHA-256 === <br>
NIST Rezultati: 3 kaluan, 0 dështuan.

=== Testimi i determinizmit === <br>
 -> Hash1: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855, Hash2: e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 | OK   <br>
abc -> Hash1: ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad, Hash2: ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad | OK  <br>
The quick brown fox jumps over the la... -> Hash1: d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592, Hash2: d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592 | OK

=== Testimi i efektit Avalanche ===  <br>
Origjinali: d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592  <br>
Modifikuari: 05c6e08f1d9fdafa03147fcb8f82f124c76d2f70e3d989dc8aadb5e7d7450bec  <br>
Numri i bitëve të ndryshëm: 142  <br>

=== Benchmarking SHA-256 ===  <br>
1 hash: 0 ms  <br>
1000 hash-e: 23 ms  <br>
Shpejtësia: 43478.26 hash/sec  <br>

=== Testimi i kolizioneve ===  <br>
Asnjë kolizion nuk u gjet.

=== Self-tests të përdoruesit për SHA-256 ===  <br>
OK   | "" -> e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  <br>
OK   | "abc" -> ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad  <br>
OK   | "The quick brown fox jumps over the la..." -> d7a8fbb307d7809469ca9abcb0082e4f8d5651e46d3cdb762d02d0bf37c9e592  <br>
OK   | "The quick brown fox jumps over the la..." -> ef537f25c895bfa782526529a9b63d97aa631564d5d789c2b765448c8635fb6c  <br>
OK   | "Hash me, Shqipëri: ëE" -> bcaa3ab6009c7c62a26b06ab7a18ad795c6e32f4421a39eabc5e85596a5b5593  <br>
Rezultati: 5 kaluan, 0 dështuan.


