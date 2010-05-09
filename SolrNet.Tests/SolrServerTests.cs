﻿#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Mapping.Validation;
using SolrNet.Schema;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrServerTests {
        [Test]
        public void Ping() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidator>();
            With.Mocks(mocks)
                .Expecting(basicServer.Ping)
                .Verify(() => {
                    var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager, null);
                    s.Ping();
                });
        }

        [Test]
        public void Commit() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidator>();
            With.Mocks(mocks)
                .Expecting(() => basicServer.Commit(null))
                .Verify(() => {
                    var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager, null);
                    s.Commit();
                });            
        }

        [Test]
        public void GetSchema() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidator>();
            With.Mocks(mocks).Expecting(() => Expect.Call(basicServer.GetSchema()).Repeat.Once().Return(new XmlDocument())).Verify(() => {
                var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager, null);
                s.GetSchema();
            });
        }

        [Test]
        public void Validate() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidator>();
            var parser = mocks.CreateMock<ISolrSchemaParser>();
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(basicServer.GetSchema()).Repeat.Once().Return(new XmlDocument());
                    Expect.Call(parser.Parse(null)).IgnoreArguments().Return(null);
                    Expect.Call(validationManager.Validate(typeof(TestDocument), new SolrSchema())).Repeat.Once().IgnoreArguments().Return(new List<MappingValidationItem>());
                })
                .Verify(() => {
                    var s = new SolrServer<TestDocument>(basicServer, mapper, validationManager, parser);
                    s.Validate().ToList();
                });
        }

        public class TestDocument {
            [SolrUniqueKey]
            public int id {
                get { return 0; }
            }
        }
    }
}